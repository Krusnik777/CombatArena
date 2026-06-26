using CombatArena.Game.Gameplay.UI;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities
{
    public interface IEffectsHolder
    {
        public UIDamageInfo UIDamageInfoPrefab { get; }
        public ParticleSystem HitEffect { get; }
        public Transform HitTransform { get; }
    }
}
