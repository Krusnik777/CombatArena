using System;
using CombatArena.Game.Gameplay.UI;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemyParticles : MonoBehaviour, IDisposable, IEffectsHolder
    {
        [field: SerializeField] public UIDamageInfo UIDamageInfoPrefab { get; private set; }
        [field: SerializeField] public ParticleSystem HitEffect { get; private set; }
        [field: SerializeField] public Transform HitTransform { get; private set; }

        public void Dispose()
        {
            
        }
    }
}
