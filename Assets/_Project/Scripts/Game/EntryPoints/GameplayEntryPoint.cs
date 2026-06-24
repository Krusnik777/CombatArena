using DI;
using CombatArena.Game.Root;
using CombatArena.Game.Services;
using UI;
using R3;
using UnityEngine;
using CombatArena.Game.Gameplay.Entities.Player;
using CombatArena.Game.Gameplay.Entities.Enemies;
using CombatArena.Game.StateMachines;
using CombatArena.Game.Gameplay.Entities.Levels;

namespace CombatArena.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint<GameplayEnterParameters,GameplayExitParameters>
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private GameplayLevelView m_levelView;
        [SerializeField] private PlayerView m_playerView;

        private GameplayStateMachine _stateMachine;

        private Subject<GameplayExitParameters> _onEnd;

        public override Observable<GameplayExitParameters> Run(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            Debug.Log("ENTRY POINT: Started Gameplay");

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
            var restartInvoker = new EventInvoker(() => Exit(new(GameplayExitTags.RESTART, enterParameters.Runs)));
            var nextInvoker = new EventInvoker(() => Exit(new(GameplayExitTags.NEXT, enterParameters.Runs + 1)));
            var exitInvoker = new EventInvoker(() => Exit(new(GameplayExitTags.EXIT, enterParameters.Runs)));

            sceneContainer.RegisterInstance(GameplayExitTags.RESTART, restartInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayExitTags.NEXT, nextInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayExitTags.EXIT, exitInvoker as IEventInvoker);

            var levelController = new GameplayLevelController(m_levelView, enterParameters.Runs);
            sceneContainer.RegisterInstance(levelController);

            var player = new Player(m_playerView, sceneContainer.Resolve<PlayerConfigsProvider>(), sceneContainer.Resolve<GameInputService>());
            sceneContainer.RegisterInstance(player);

            sceneContainer.RegisterFactory(c => new EnemyFactory(c.Resolve<EnemyConfigsProvider>().EnemiesCollection)).AsSingle();
        }

        private void SetupUI(DIContainer sceneContainer)
        {
            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var uiSceneRoot = Instantiate(m_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiSceneRoot.gameObject);

            var windowsFactory = new GameplayWindowsFactory(uiSceneRoot.ScreensTransform, uiSceneRoot.PopupsTransform);
            sceneContainer.RegisterInstance(new UIWindowsProvider(windowsFactory));
            //sceneContainer.RegisterFactory(_ => new UIWindowsProvider(windowsFactory)).AsSingle();
        }
    }
}
