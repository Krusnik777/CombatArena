using CombatArena.Game.Root;
using CombatArena.Game.Services;
using R3;
using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public class DefeatScreen : Screen
    {
        public Observable<string> OnChoseMade => _onChoseMade;

        private DefeatScreenView _concreteView => _view as DefeatScreenView;
        private GameInputService _bindedGameInputService;

        private Subject<string> _onChoseMade = new();

        private CompositeDisposable _buttonListenerDisposables;

        public DefeatScreen(DefeatScreenView view) : base(view) { }

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

        private void SubscribeToButtons()
        {
            _buttonListenerDisposables?.Dispose();

            _buttonListenerDisposables = new()
            {
                _concreteView.OnRestartPress.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.RESTART)),
                _concreteView.OnGiveUpPress.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.EXIT))
            };

            if (_bindedGameInputService != null)
            {
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnSubmitPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.RESTART)));
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnCancelPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.EXIT)));
            }
        }
    }
}
