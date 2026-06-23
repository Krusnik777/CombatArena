using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.UI;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public EnemyConfig Config { get; private set; } 
        [field: SerializeField] public Damageable Damageable { get; private set; }
        [field: SerializeField] public Transform ViewTransform { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public AnimatorEventsCollector EventsCollector { get; private set; }
        [field: SerializeField] public UIHealth UIHealth { get; private set; }
    }
}
