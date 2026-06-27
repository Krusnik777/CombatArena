using CombatArena.Game.Services;
using DG.Tweening;
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
            //if (_isShowing) SubscribeToButtons();
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

            _concreteView.Title.alpha = 0f;
            _concreteView.Message.alpha = 0f;
            _concreteView.ButtonsContainer.SetActive(false);

            var anim = DOTween.Sequence();
            anim.AppendInterval(1f);
            anim.Append(_concreteView.Title.DOFade(1f, 1f).SetEase(Ease.InSine));
            anim.Append(_concreteView.Message.DOFade(1f, 1f).SetEase(Ease.InSine));
            anim.AppendInterval(1f);
            anim.OnComplete(() =>
            {
                SubscribeToButtons();
                _concreteView.ButtonsContainer.SetActive(true);
            });
            anim.SetLink(_concreteView.gameObject);

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
