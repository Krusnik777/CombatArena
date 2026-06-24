using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public class BeforeBattleScreen : Screen
    {
        private BeforeBattleScreenView _concreteView => _view as BeforeBattleScreenView;

        public BeforeBattleScreen(BeforeBattleScreenView view) : base(view) { }
    }
}
