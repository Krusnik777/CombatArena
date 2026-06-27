using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerView : MonoBehaviour
    {
        [field: SerializeField] public Damageable Damageable { get; private set; }
        [field: SerializeField] public PlayerAvatarMovement Movement { get; private set; }
        [field: SerializeField] public PlayerAvatarAnimator Animator { get; private set; }
        [field: SerializeField] public AnimatorEventsCollector EventsCollector { get; private set; }
        [field: SerializeField] public Transform ShealteredSwordTransform { get; private set; }
        [field: SerializeField] public Transform SwordTransform { get; private set; }
        [field: SerializeField] public PlayerParticles Particles { get; private set; }
        [field: SerializeField] public DashEffects DashEffects { get; private set; }
    }
}
