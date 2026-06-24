using UnityEngine;
using UnityEngine.AI;

namespace CombatArena.Game.Gameplay.Entities.Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        private const string _isMoving = "IsMoving";
        private const string _isAttackTrigger = "Attack";
        private const float _movementThreshold = 0.05f;

        [SerializeField] private Animator m_animator;
        [SerializeField] private NavMeshAgent m_agent;

        public void PlayAttack()
        {
            m_animator.SetTrigger(_isAttackTrigger);
        }

        private void Update()
        {
            m_animator.SetBool(_isMoving, m_agent.velocity.magnitude >= _movementThreshold);
        }
    }
}
