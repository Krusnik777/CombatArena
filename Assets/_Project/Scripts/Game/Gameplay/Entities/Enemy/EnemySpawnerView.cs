using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemySpawnerView : MonoBehaviour
    {
        [field: SerializeField] public Transform SpawnPoint { get; private set; }
        [field: SerializeField] public bool SpawnAtStart { get; private set; } = true;
        [field: SerializeField] public int SecondsUntilNextSpawn { get; private set; } = 20;
        [field: SerializeField] public float AvailableCheckRange { get; private set; } = 5f;
        [field: SerializeField] public ParticleSystem Effect { get; private set; }
    }
}
