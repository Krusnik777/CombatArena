using UnityEngine;
using UnityEngine.AI;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;
        [SerializeField] private NavMeshAgent m_agent;

        private const string _isMovingBool = "IsMoving";
        private const string _attackTrigger = "Attack";
        private const string _deathTrigger = "Die";
        private const float _movementThreshold = 0.05f;

        public void ResetControllerState()
        {
            m_animator.ResetControllerState();
        }

        public void PlayAttack()
        {
            m_animator.SetTrigger(_attackTrigger);
        }

        public void PlayDeath()
        {
            m_animator.SetTrigger(_deathTrigger);
        }

        private void Update()
        {
            m_animator.SetBool(_isMovingBool, m_agent.velocity.magnitude >= _movementThreshold);
        }
    }
}
