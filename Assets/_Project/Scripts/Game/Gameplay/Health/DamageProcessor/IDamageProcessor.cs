using UnityEngine;

namespace CombatArena.Game.Gameplay.HealthSystem
{
    public interface IDamageProcessor
    {
        public void Process(ref Damage damage);
    }
}
