using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarAnimator : MonoBehaviour
    {
        private const string _isMoving = "IsMoving";
        private const string _simpleAttack = "SimpleAttack";
        private const string _superAttack = "SuperAttack";
        private const float _movementThreshold = 0.05f;

        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Animator m_animator;

        private PlayerAvatarMovement _movement;

        public void Bind(PlayerAvatarMovement movement)
        {
            _movement = movement;
        }

        private void Update()
        {
            if (_movement != null)
            {
                m_animator.SetBool(_isMoving, _movement.DirectionControl.magnitude >= _movementThreshold);
            }
            else
            {
                m_animator.SetBool(_isMoving, m_characterController.velocity.magnitude >= _movementThreshold);
            }
        }

        public void PlaySimpleAttack()
        {
            m_animator.SetTrigger(_simpleAttack);
        }

        public void PlaySuperAttack()
        {
            m_animator.SetTrigger(_superAttack);
        }
    }
}