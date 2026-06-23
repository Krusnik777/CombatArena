using CombatArena.Game.Gameplay.Entities.Player;
using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public class BattleScreen : Screen
    {
        private BattleScreenView _concreteView => _view as BattleScreenView;

        public BattleScreen(BattleScreenView view) : base(view) { }

        public override void Dispose()
        {
            base.Dispose();

            _concreteView.UIPlayerHealth.Dispose();

            _concreteView.UIAbilityA.Dispose();
            _concreteView.UIAbilityX.Dispose();
            _concreteView.UIAbilityY.Dispose();
        }

        public void Initialize(Player player)
        {
            _concreteView.UIPlayerHealth.Bind(player.Health);

            _concreteView.UIAbilityA.Bind(player.Abilities.AbilityA);
            _concreteView.UIAbilityX.Bind(player.Abilities.AbilityX);
            _concreteView.UIAbilityY.Bind(player.Abilities.AbilityY);
        }
    }
}
