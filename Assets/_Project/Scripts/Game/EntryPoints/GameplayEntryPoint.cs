using DI;
using CombatArena.Game.Root;
using CombatArena.Game.Services;
using CombatArena.Game.Gameplay.Entities.Player;
using CombatArena.Game.Gameplay.Entities.Enemies;
using CombatArena.Game.StateMachines;
using CombatArena.Game.Gameplay.Entities.Levels;
using UI;
using R3;
using UnityEngine;


namespace CombatArena.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint<GameplayEnterParameters,GameplayExitParameters>
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private GameplayLevelView m_levelView;
        [SerializeField] private PlayerView m_playerView;
        [SerializeField] private PlayerCamera m_playerCamera;
        [Header("Pools Transforms")]
        [SerializeField] private Transform m_particlesTransform;
        [SerializeField] private Transform m_enemiesTransform;

        private GameplayStateMachine _stateMachine;

        private Subject<GameplayExitParameters> _onEnd;

        public override Observable<GameplayExitParameters> Run(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            _onEnd = new();

            RegisterLocalInstances(sceneContainer, enterParameters);
            SetupUI(sceneContainer);

            _stateMachine = new GameplayStateMachine(sceneContainer);
            _stateMachine.SetState<BeforeBattleState>();

            return _onEnd;
        }

        private void OnDestroy()
        {
            DisposeOfListeners();
        }

        private void Exit(GameplayExitParameters exitParameters)
        {
            DisposeOfListeners();

            _onEnd.OnNext(exitParameters);
        }

        private void DisposeOfListeners()
        {
            _stateMachine?.Dispose();
        }

        private void RegisterLocalInstances(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            var restartInvoker = new EventInvoker(() => Exit(new(GameplayTags.RESTART, enterParameters.Runs)));
            var nextInvoker = new EventInvoker(() => Exit(new(GameplayTags.NEXT, enterParameters.Runs + 1)));
            var exitInvoker = new EventInvoker(() => Exit(new(GameplayTags.EXIT, enterParameters.Runs)));

            sceneContainer.RegisterInstance(GameplayTags.RESTART, restartInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.NEXT, nextInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.EXIT, exitInvoker as IEventInvoker);

            var particlesPool = new SimpleGameObjectsPool(m_particlesTransform);
            sceneContainer.RegisterInstance(GameplayTags.ParticlesPool, particlesPool);

            var levelController = new GameplayLevelController(m_levelView, enterParameters.Runs);
            var enemyPool = new EnemyPool(sceneContainer.Resolve<EnemyConfigsProvider>().EnemiesCollection, sceneContainer.Resolve<AudioService>().Sounds, m_enemiesTransform);
            levelController.PrepareSpawners(enemyPool, particlesPool);
            sceneContainer.RegisterInstance(levelController);

            var player = new Player(m_playerView, sceneContainer);
            m_playerView.Particles.Initialize(m_playerView.EventsCollector, particlesPool);
            sceneContainer.RegisterInstance(player);
            var abilitiesProvider = sceneContainer.Resolve<AbilityConfigsProvider>();
            var playerAbilitiesFactory = new Gameplay.PlayerAbilitiesFactory(abilitiesProvider.AbilitiesCollection);
            var playerAbilities = playerAbilitiesFactory.Create(player);
            sceneContainer.RegisterInstance(playerAbilities);

            sceneContainer.RegisterInstance(m_playerCamera);
        }

        private void SetupUI(DIContainer sceneContainer)
        {
            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var uiSceneRoot = Instantiate(m_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiSceneRoot.gameObject);

            var windowsFactory = new GameplayWindowsFactory(uiSceneRoot.ScreensTransform, uiSceneRoot.PopupsTransform);
            sceneContainer.RegisterInstance(new UIWindowsProvider(windowsFactory));
            //sceneContainer.RegisterFactory(_ => new UIWindowsProvider(windowsFactory)).AsSingle();

            var tooltipsFactory = new TooltipsFactory(uiSceneRoot.TooltipsCanvas.transform);
            var tooltipsHandler = new TooltipsService(uiSceneRoot.TooltipsCanvas, uiSceneRoot.TooltipOffset, tooltipsFactory);
            sceneContainer.RegisterInstance(tooltipsHandler as UI.Tooltips.ITooltipHandler);
        }
    }
}
