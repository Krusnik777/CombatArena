using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera m_cinemachineCamera;
        [SerializeField] private CinemachineFollow m_cinemachineFollow;
        [SerializeField] private float m_defaultOrtographicSize = 3.75f;
        [SerializeField] private float m_defaultYFollowOffset = 2f;
        [SerializeField] private float m_eventsOrtographicSize = 2f;
        [SerializeField] private float m_eventsYFollowOffset = 1.25f;

        private Sequence _cameraChangeSequence;

        public void SetDefaultView(float duration = 1f) => ChangeOrtographicSize(m_defaultOrtographicSize, m_defaultYFollowOffset, duration);
        public void SetEventsView(float duration = 1f) => ChangeOrtographicSize(m_eventsOrtographicSize, m_eventsYFollowOffset, duration);

        private void ChangeOrtographicSize(float targetOrtographicSize, float targetYOffset, float duration)
        {
            _cameraChangeSequence?.Kill();
            
            float startOrtographicSize = m_cinemachineCamera.Lens.OrthographicSize;
            float startYOffset = m_cinemachineFollow.FollowOffset.y;

            _cameraChangeSequence = DOTween.Sequence();
            _cameraChangeSequence.Append(DOVirtual.Float(startOrtographicSize, targetOrtographicSize, duration, (value) =>
            {
                m_cinemachineCamera.Lens.OrthographicSize = value;
            }));
            _cameraChangeSequence.Join(DOVirtual.Float(startYOffset, targetYOffset, duration, (value) =>
            {
                m_cinemachineFollow.FollowOffset.y = value;
            }));
            _cameraChangeSequence.SetLink(gameObject);
        }
    }
}
