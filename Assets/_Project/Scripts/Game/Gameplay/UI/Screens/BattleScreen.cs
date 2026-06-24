using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.Entities.Player;
using R3;
using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public class BattleScreen : Screen
    {
        private BattleScreenView _concreteView => _view as BattleScreenView;

        private System.IDisposable _enemiesRemainedListenerDisposable;

        public BattleScreen(BattleScreenView view) : base(view) { }

        public override void Dispose()
        {
            base.Dispose();

            _concreteView.UIPlayerHealth.Dispose();

            _concreteView.UIAbilityA.Dispose();
            _concreteView.UIAbilityX.Dispose();
            _concreteView.UIAbilityY.Dispose();

            _enemiesRemainedListenerDisposable?.Dispose();
        }

        public void Initialize(Player player, GameplayLevelController levelController)
        {
            _concreteView.UIPlayerHealth.Bind(player.Health);

            _concreteView.UIAbilityA.Bind(player.Abilities.AbilityA);
            _concreteView.UIAbilityX.Bind(player.Abilities.AbilityX);
            _concreteView.UIAbilityY.Bind(player.Abilities.AbilityY);

            _enemiesRemainedListenerDisposable = levelController.OnEnemyDied.Subscribe(remainingEnemiesCount =>
            {
                var count = remainingEnemiesCount < 0 ? 0 : remainingEnemiesCount;
                _concreteView.EnemiesRemainingText.text = $"Remaining Enemies: {count}";
            });
        }
    }
}
