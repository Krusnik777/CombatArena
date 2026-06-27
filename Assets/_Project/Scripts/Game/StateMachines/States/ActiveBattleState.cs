using CombatArena.Game.Gameplay;
using CombatArena.Game.Gameplay.Entities.Enemies;
using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.Entities.Player;
using CombatArena.Game.Gameplay.UI;
using CombatArena.Game.Services;
using DI;
using R3;
using StateMachine;

namespace CombatArena.Game.StateMachines
{
    public class ActiveBattleState : IEnterableState
    {
        private IStateMachine _stateMachine;
        private DIContainer _sceneContainer;

        private System.IDisposable _enemiesDeathsListenerDisposable;
        private System.IDisposable _playerDeathListenerDisposable;

        public ActiveBattleState(IStateMachine stateMachine, DIContainer sceneContainer)
        {
            _stateMachine = stateMachine;
            _sceneContainer = sceneContainer;
        }

        public void Enter()
        {
            var player = _sceneContainer.Resolve<Player>();
            player.AssignAbilities(_sceneContainer.Resolve<PlayerAbilities>());

            var levelController = _sceneContainer.Resolve<GameplayLevelController>();
            levelController.AssignEnemyDetector(player.EnableEnemyDetector());
            levelController.SetEnterGateEnabled(true);
            levelController.StartSpawners(player.Transform);

            _sceneContainer.Resolve<AudioService>().Music.Play(true);

            var screen = _sceneContainer.Resolve<UIWindowsProvider>().ShowScreen<BattleScreen>();
            screen.Initialize(player, levelController);
            screen.BindTooltipHandler(_sceneContainer.Resolve<UI.Tooltips.ITooltipHandler>());

            _enemiesDeathsListenerDisposable = levelController.OnEnemyDied.Where(enemiesRemained => enemiesRemained <= 0).Subscribe(_ =>
            {
                _enemiesDeathsListenerDisposable?.Dispose();
                _playerDeathListenerDisposable?.Dispose();

                _stateMachine.SetState<BattleResultState, bool>(true);
            });

            _playerDeathListenerDisposable = player.OnDeath.Subscribe(_ =>
            {
                _enemiesDeathsListenerDisposable?.Dispose();
                _playerDeathListenerDisposable?.Dispose();

                _stateMachine.SetState<BattleResultState, bool>(false);
            });
        }

        public void Exit()
        {
            _enemiesDeathsListenerDisposable?.Dispose();
            _playerDeathListenerDisposable?.Dispose();
        }
    }
}
