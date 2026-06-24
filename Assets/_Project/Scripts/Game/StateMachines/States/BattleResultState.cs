using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.Entities.Player;
using CombatArena.Game.Gameplay.UI;
using CombatArena.Game.Root;
using CombatArena.Game.Services;
using DI;
using R3;
using StateMachine;

namespace CombatArena.Game.StateMachines
{
    public class BattleResultState : IEnterableState<bool>
    {
        private IStateMachine _stateMachine;
        private DIContainer _sceneContainer;

        private System.IDisposable _disposable;

        public BattleResultState(IStateMachine stateMachine, DIContainer sceneContainer)
        {
            _stateMachine = stateMachine;
            _sceneContainer = sceneContainer;
        }

        public void Enter(bool isVictory)
        {
            var player = _sceneContainer.Resolve<Player>();
            player.Stop();

            var levelController = _sceneContainer.Resolve<GameplayLevelController>();
            levelController.StopSpawnersAndEnemies();
            // LATER: Play victory music or fail music

            var gameInputService = _sceneContainer.Resolve<GameInputService>();

            var windowsProvider = _sceneContainer.Resolve<UIWindowsProvider>();

            if (isVictory)
            {
                var window = windowsProvider.ShowScreen<VictoryScreen>();
                window.Bind(gameInputService);

                _disposable = window.OnChoseMade.Subscribe(resultTag =>
                {
                    var invoker = _sceneContainer.Resolve<IEventInvoker>(resultTag);
                    invoker.InvokeBindedAction();
                });
            }
            else
            {
                var window = windowsProvider.ShowScreen<DefeatScreen>();
                window.Bind(gameInputService);

                _disposable = window.OnChoseMade.Subscribe(resultTag =>
                {
                    var invoker = _sceneContainer.Resolve<IEventInvoker>(resultTag);
                    invoker.InvokeBindedAction();
                });
            }

            
        }

        public void Exit()
        {
            _disposable?.Dispose();
        }
    }
}
