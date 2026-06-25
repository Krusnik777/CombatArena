using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Loading;
using R3;
using DI;
using CombatArena.Game.EntryPoints;
using CombatArena.Game.Services;

namespace CombatArena.Game.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private readonly UIRootView _uiRoot;
        private readonly LoadingManager _loadingManager;
        
        private readonly DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;

        private bool _errorLoadingImitationWasExperienced = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot);

            _loadingManager = new(_uiRoot.LoadingScreen);

            SetupAudioService();
            SetupInputServices();
            SetupProviders();
        }

        private void RunGame()
        {
            #if UNITY_EDITOR

            var sceneName = _loadingManager.GetActiveSceneName();

            if (sceneName == Scenes.GAMEPLAY)
            {
                LoadAndStartGameplay(new GameplayEnterParameters(0), true);

                return;
            }

            if (sceneName != Scenes.BOOTSTRAP)
            {
                return;
            }
            
            #endif

            LoadAndStartGameplay(new GameplayEnterParameters(0), true);
        }

        private void LoadAndStartGameplay(GameplayEnterParameters enterParams, bool isFromBootstrap = false)
        {
            _cachedSceneContainer?.Dispose();

            var someFakeAwaitableService = new FakeAwaitableService();

            var sometimesFailingLoadingStep = someFakeAwaitableService.CreateSometimesFailingLoadingStep("Imitating Sometimes Failing Service", 1000, () => {}, 
                () => _errorLoadingImitationWasExperienced = true, isFromBootstrap || _errorLoadingImitationWasExperienced ? 0.5f : 1f);

            List<LoadingStep> steps = new()
            {
                _loadingManager.CreateSceneLoadingStep(Scenes.BOOTSTRAP, isFromBootstrap ? "Initializing Global Services..." : "Scene Cleanup..."),
                someFakeAwaitableService.CreateLoadingStep("Imitating Some Service Initialization...", 250, () => {}),
                someFakeAwaitableService.CreateLoadingStep("Imitating Another Service Initialization...", 250, () => {}),
                sometimesFailingLoadingStep,
                someFakeAwaitableService.CreateLoadingStep("Imitating One More Service Initialization...", 250, () => {})
            };

            var finalLoadingStep = _loadingManager.CreateWaitingLoadingStep("Final Scene Setup...", 250, () => HandleGameplayEntryPoint(enterParams));
            _loadingManager.LoadScene(Scenes.GAMEPLAY, steps, finalLoadingStep);
        }

        private void HandleGameplayEntryPoint(GameplayEnterParameters enterParams)
        {
            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint<GameplayEnterParameters, GameplayExitParameters>>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer, enterParams).Subscribe(exitParameters =>
            {
                if (exitParameters.ExitTag == GameplayExitTags.RESTART)
                {
                    LoadAndStartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayExitTags.NEXT)
                {
                    LoadAndStartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayExitTags.EXIT)
                {
                    #if UNITY_EDITOR
                    LoadAndStartGameplay(new GameplayEnterParameters(0));
                    #else
                    Application.Quit();
                    #endif

                    return;
                }

                throw new System.NotImplementedException("[Gameplay Exit Parameters] Current exit parameters currently not supported");
            });
        }

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
