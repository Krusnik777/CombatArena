using CombatArena.Game.Gameplay.UI;
using UnityEngine;
using UnityEngine.AI;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Damageable Damageable { get; private set; }
        [field: SerializeField] public Transform ViewTransform { get; private set; }
        [field: SerializeField] public EnemyAnimator Animator { get; private set; }
        [field: SerializeField] public AnimatorEventsCollector EventsCollector { get; private set; }
        [field: SerializeField] public UIHealth UIHealth { get; private set; }
        [field: SerializeField] public GameObject ChosenEffect { get; private set; }
        [field: SerializeField] public EnemyParticles Particles { get; private set; }
    }
}
