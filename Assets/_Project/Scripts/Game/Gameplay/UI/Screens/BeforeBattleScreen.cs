using DG.Tweening;
using UnityEngine;
using Screen = UI.Windows.Screen;

namespace CombatArena.Game.Gameplay.UI
{
    public class BeforeBattleScreen : Screen
    {
        private BeforeBattleScreenView _concreteView => _view as BeforeBattleScreenView;

        public BeforeBattleScreen(BeforeBattleScreenView view) : base(view) { }

        public override void Show()
        {
            base.Show();

            _concreteView.FadeImage.gameObject.SetActive(true);
            _concreteView.Text.alpha = 0f;

            for (int i = 0; i < _concreteView.ScreenBorders.Length; i++)
            {
                _concreteView.ScreenBorders[i].localScale = new Vector3(1, 1, 1);
            }
        }

        public void DoFadeIn(float duration, System.Action onEnd)
        {
            if (duration <= 0f) return;

            _concreteView.FadeImage.color = Color.black;
            var color = Color.black;
            color.a = 0f;

            _concreteView.FadeImage.gameObject.SetActive(true);

            var anim = _concreteView.FadeImage.DOColor(color, duration).SetEase(Ease.InSine).OnComplete(() =>
            {
                _concreteView.FadeImage.gameObject.SetActive(false);

                ShowTextAndHideBorders();

                onEnd?.Invoke();
            });
            anim.SetLink(_concreteView.gameObject);
        }

        private void FadeOut(float duration, System.Action onEnd)
        {
            if (duration <= 0f) return;

            var color = Color.black;
            color.a = 0f;
            _concreteView.FadeImage.color = color;

            _concreteView.FadeImage.gameObject.SetActive(true);

            var anim = _concreteView.FadeImage.DOColor(Color.black, duration).SetEase(Ease.InSine).OnComplete(() =>
            {
                _concreteView.FadeImage.gameObject.SetActive(false);
                onEnd?.Invoke();
            });
            anim.SetLink(_concreteView.gameObject);
        }

        private void ShowTextAndHideBorders()
        {
            var textAnim = _concreteView.Text.DOFade(1f, 0.5f).SetEase(Ease.InSine);
            textAnim.SetLink(_concreteView.gameObject);

            for (int i = 0; i < _concreteView.ScreenBorders.Length; i++)
            {
                var anim = _concreteView.ScreenBorders[i].DOScaleY(0, 0.5f).SetEase(Ease.InSine);
                anim.SetLink(_concreteView.gameObject);
            }
        }
    }
}
