using CombatArena.Game.Services;
using R3;
using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public abstract class ResultScreen : Screen
    {
        public Observable<string> OnChoseMade => _onChoseMade;

        private ResultScreenView _concreteView => _view as ResultScreenView;
        protected GameInputService _bindedGameInputService;

        protected Subject<string> _onChoseMade = new();

        protected CompositeDisposable _buttonListenerDisposables;

        public ResultScreen(ResultScreenView view) : base(view) { }

        public void Bind(GameInputService gameInputService)
        {
            _bindedGameInputService = gameInputService;
            if (_isShowing) SubscribeToButtons();
        }

        public override void Dispose()
        {
            base.Dispose();

            _buttonListenerDisposables?.Dispose();
            for (int i = 0; i < _concreteView.ControlsTips.Length; i++) _concreteView.ControlsTips[i].Dispose();
        }

        public override void Show()
        {
            base.Show();

            SubscribeToButtons();

            for (int i = 0; i < _concreteView.ControlsTips.Length; i++) _concreteView.ControlsTips[i].Initialize();
        }

        public override void Hide()
        {
            base.Hide();

            _buttonListenerDisposables?.Dispose();
            for (int i = 0; i < _concreteView.ControlsTips.Length; i++) _concreteView.ControlsTips[i].Dispose();
        }

        protected abstract void SubscribeToButtons();
    }
}
