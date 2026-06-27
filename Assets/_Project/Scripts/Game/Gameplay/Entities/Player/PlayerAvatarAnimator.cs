using System.Collections;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarAnimator : MonoBehaviour
    {
        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Animator m_animator;

        private const string _isMovingBool = "IsMoving";

        private const string _simpleAttackTrigger = "SimpleAttack";
        private const string _superAttackTrigger = "SuperAttack";
        private const string _deathTrigger = "Die";
        private const string _equipTrigger = "Equip";
        private const string _calmTrigger = "Calm";
        private const string _winTrigger = "Win";

        private const float _movementThreshold = 0.05f;

        private PlayerAvatarMovement _movement;
        private bool _isActive = true;

        public void Bind(PlayerAvatarMovement movement)
        {
            _movement = movement;
        }

        public void SetMovementAnimationActive(bool state) => _isActive = state;

        private void Update()
        {
            if (_movement != null)
            {
                m_animator.SetBool(_isMovingBool, _movement.DirectionControl.magnitude >= _movementThreshold && _isActive);
            }
            else
            {
                m_animator.SetBool(_isMovingBool, m_characterController.velocity.magnitude >= _movementThreshold && _isActive);
            }
        }

        public void PlaySimpleAttack()
        {
            m_animator.SetTrigger(_simpleAttackTrigger);
        }

        public void PlaySuperAttack()
        {
            m_animator.SetTrigger(_superAttackTrigger);
        }

        public void PlayDeath()
        {
            m_animator.SetTrigger(_deathTrigger);
        }

        public void PlayWin()
        {
            m_animator.SetTrigger(_winTrigger);
        }

        public void SetAsCalm()
        {
            m_animator.SetTrigger(_calmTrigger);
        }

        public void PlayEquip(System.Action onEnd)
        {
            m_animator.SetTrigger(_equipTrigger);

            StartCoroutine(EndEquipAnimationRoutine(onEnd));
        }

        private IEnumerator EndEquipAnimationRoutine(System.Action onEnd)
        {
            yield return new WaitForSeconds(1.5f);

            onEnd?.Invoke();
        }
    }
}