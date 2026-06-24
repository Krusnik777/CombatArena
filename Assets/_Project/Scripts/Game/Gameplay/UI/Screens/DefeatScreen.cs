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
        }

        public override void Show()
        {
            base.Show();

            SubscribeToButtons();
        }

        public override void Hide()
        {
            base.Hide();

            _buttonListenerDisposables?.Dispose();
        }

        private void SubscribeToButtons()
        {
            _buttonListenerDisposables?.Dispose();

            _buttonListenerDisposables = new()
            {
                _concreteView.OnRestartPress.Subscribe(_ => _onChoseMade.OnNext(GameplayExitTags.RESTART)),
                _concreteView.OnGiveUpPress.Subscribe(_ => _onChoseMade.OnNext(GameplayExitTags.EXIT))
            };

            if (_bindedGameInputService != null)
            {
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnSubmitPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayExitTags.RESTART)));
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnCancelPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayExitTags.EXIT)));
            }
        }
    }
}
