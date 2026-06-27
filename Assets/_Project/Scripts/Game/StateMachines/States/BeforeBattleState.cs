using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.Entities.Player;
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
            var player = _sceneContainer.Resolve<Player>();
            player.SetActive(false);
            
            var levelController = _sceneContainer.Resolve<GameplayLevelController>();
            levelController.SetEnterGateEnabled(false);

            _sceneContainer.Resolve<PlayerCamera>().SetEventsView();

            // LATER: SetPlayer As Not battle ready
            _sceneContainer.Resolve<AudioService>().Music.Play(false);

            var window = _sceneContainer.Resolve<UIWindowsProvider>().ShowScreen<BeforeBattleScreen>();
            window.DoFadeIn(2f, () => player.SetActive(true));

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
