using CombatArena.Game.Gameplay.Entities.Enemies;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Levels
{
    public class GameplayLevelView : MonoBehaviour
    {
        [field: SerializeField] public EnterTrigger ArenaEnterTrigger { get; private set; }
        [field: SerializeField] public EnemySpawnerView[] EnemySpawnerViews { get; private set; }
        [field: SerializeField] public GameObject EnterGate { get; private set; }
    }
}
