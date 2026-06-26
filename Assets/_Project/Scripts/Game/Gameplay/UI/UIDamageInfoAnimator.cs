using DG.Tweening;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIDamageInfoAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private float m_flyHeight = 2f;
        [SerializeField] private float m_duration = 0.8f;
        [SerializeField] private float m_fadeStartRatio = 0.33f;

        public Subject<UIDamageInfo> OnEnd { get; private set; } = new();

        private Sequence _currentSequence;

        public void ResetState()
        {
            _currentSequence?.Kill();
            _currentSequence = null;

            m_canvasGroup.alpha = 0f;
            transform.localPosition = Vector3.zero;
        }

        public void Play(UIDamageInfo sender)
        {
            ResetState();

            float randomOffsetX = Random.Range(-0.4f, 0.4f);
            float randomOffsetZ = Random.Range(-0.4f, 0.4f);

            Vector3 startPos = transform.localPosition;
            Vector3 targetPos = startPos + new Vector3(randomOffsetX, m_flyHeight, randomOffsetZ);

            _currentSequence = DOTween.Sequence();
            _currentSequence.SetLink(gameObject);

            _currentSequence.Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
            _currentSequence.Append(transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack));

            _currentSequence.Join(m_canvasGroup.DOFade(1f, 0.1f));

            _currentSequence.Append(transform.DOLocalMove(targetPos, m_duration).SetEase(Ease.Linear));

            float fadeDelay = m_duration * m_fadeStartRatio;
            float fadeDuration = m_duration - fadeDelay;
            _currentSequence.Insert(fadeDelay, m_canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear));

            _currentSequence.AppendInterval(0.5f);

            _currentSequence.OnComplete(() =>
            {
                OnEnd?.OnNext(sender);
            });
        }

        private void OnDisable()
        {
            _currentSequence?.Kill();
            _currentSequence = null;
        }
    }
}
