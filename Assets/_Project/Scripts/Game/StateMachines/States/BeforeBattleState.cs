using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.UI;
using CombatArena.Game.Services;
using DI;
using R3;
using StateMachine;

namespace CombatArena.Game.StateMachines
{
    public class BeforeBattleState : IEnterableState
    {
        private IStateMachine _stateMachine;
        private DIContainer _sceneContainer;

        private System.IDisposable _disposable;

        public BeforeBattleState(IStateMachine stateMachine, DIContainer sceneContainer)
        {
            _stateMachine = stateMachine;
            _sceneContainer = sceneContainer;
        }

        public void Enter()
        {
            var levelController = _sceneContainer.Resolve<GameplayLevelController>();
            levelController.SetEnterGateEnabled(false);

            // LATER: SetPlayer As Not battle ready
            _sceneContainer.Resolve<AudioService>().Music.Play(false);

            _sceneContainer.Resolve<UIWindowsProvider>().ShowScreen<BeforeBattleScreen>();

            _disposable = levelController.OnPlayerEnteredToArena().Subscribe(_ =>
            {
                _stateMachine.SetState<ActiveBattleState>();
            });
        }

        public void Exit()
        {
            _disposable?.Dispose();
        }
    }
}
