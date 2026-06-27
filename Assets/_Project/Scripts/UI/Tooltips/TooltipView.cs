using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Tooltips
{
    public class TooltipView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private TMP_Text m_info;

        private Tween _showAnimation;
        private Tween _hideAnimation;

        public void Setup(TooltipData data)
        {
            m_title.text = data.Title;
            if (m_description != null) m_description.text = data.Description;
            if (m_info != null) m_info.text = data.Info;
        }

        public void Show()
        {
            _showAnimation?.Kill();
            _hideAnimation?.Kill();

            m_canvasGroup.alpha = 0f;
            gameObject.SetActive(true);

            _showAnimation = m_canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.InSine);
            _showAnimation.SetLink(gameObject);
        }

        public void Hide()
        {
            _hideAnimation?.Kill();
            _showAnimation?.Kill();

            m_canvasGroup.alpha = 1f;

            _hideAnimation = m_canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InSine).OnComplete(() => gameObject.SetActive(false));
            _hideAnimation.SetLink(gameObject);
        }
    }
}
