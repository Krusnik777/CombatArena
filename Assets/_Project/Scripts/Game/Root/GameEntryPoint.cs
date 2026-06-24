using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using R3;
using DI;
using Utils;
using CombatArena.Game.EntryPoints;
using CombatArena.Game.Services;

namespace CombatArena.Game.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private Coroutines _coroutines;
        private UIRootView _uiRoot;

        private readonly DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot);

            SetupAudioService();
            SetupInputServices();
            SetupProviders();
        }

        private /*async*/ void RunGame()
        {
            #if UNITY_EDITOR

            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == Scenes.GAMEPLAY)
            {
                StartGameplay(new GameplayEnterParameters(0));

                return;
            }

            if (sceneName != Scenes.BOOTSTRAP)
            {
                return;
            }
            
            #endif

            StartGameplay(new GameplayEnterParameters(0));
        }

        private void StartGameplay(GameplayEnterParameters enterParams) => _coroutines.StartCoroutine(LoadAndStartGameplay(enterParams));

        #region Routines

        private IEnumerator LoadAndStartGameplay(GameplayEnterParameters enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOTSTRAP);
            yield return LoadScene(Scenes.GAMEPLAY);

            yield return new WaitForSeconds(1);

            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint<GameplayEnterParameters,GameplayExitParameters>>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer, enterParams).Subscribe(exitParameters =>
            {
                if (exitParameters.ExitTag == GameplayExitTags.RESTART)
                {
                    StartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayExitTags.NEXT)
                {
                    StartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayExitTags.EXIT)
                {

                    #if UNITY_EDITOR

                    StartGameplay(new GameplayEnterParameters(0));

                    #else

                    Application.Quit();
                    
                    #endif

                    return;
                }

                throw new System.NotImplementedException("[Gameplay Exit Parameters] Current exit parameters currently not supported");
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        #endregion

        #region Services Setup Methods

        private void SetupAudioService()
        {
            var mixer = Resources.Load<AudioMixer>("AudioMixer");
            var sfxGroup = mixer.FindMatchingGroups("SFX")[0];
            var bgmGroup = mixer.FindMatchingGroups("BGM")[0];

            var audioSystemContainer = new GameObject("[AUDIO]").AddComponent<AudioListener>();

            var soundsContainer = new GameObject("[SOUNDS]").AddComponent<AudioSource>();
            soundsContainer.outputAudioMixerGroup = sfxGroup;
            soundsContainer.transform.SetParent(audioSystemContainer.transform);
            //var loopSoundsContainer = new GameObject("[SOUNDS_LOOP]").AddComponent<AudioSource>();
            //loopSoundsContainer.outputAudioMixerGroup = sfxGroup;
            //loopSoundsContainer.transform.SetParent(audioSystemContainer.transform);

            AudioSource bgmContainer = new GameObject("[BACKGROUND_MUSIC]").AddComponent<AudioSource>();
            bgmContainer.outputAudioMixerGroup = bgmGroup;
            bgmContainer.transform.SetParent(audioSystemContainer.transform);

            Object.DontDestroyOnLoad(audioSystemContainer);

            // AudioService init
        }

        private void SetupInputServices()
        {
            var inputDeviceDetectService = new InputDeviceDetectService();
            _rootContainer.RegisterInstance(inputDeviceDetectService);

            var gameInputService = new GameInputService();
            _rootContainer.RegisterInstance(gameInputService);
        }

        private void SetupProviders()
        {
            var playerAvatarConfigProvider = new PlayerConfigsProvider();
            _rootContainer.RegisterInstance(playerAvatarConfigProvider);

            var abilityConfigsProvider = new AbilityConfigsProvider();
            _rootContainer.RegisterInstance(abilityConfigsProvider);

            var enemyConfigsProvider = new EnemyConfigsProvider();
            _rootContainer.RegisterInstance(enemyConfigsProvider);

            var damageableMasksLoader = new DamageableMasksLoader();
            damageableMasksLoader.LoadMasksAndAssignToLayerMasks();
        }

        #endregion
    }
}
