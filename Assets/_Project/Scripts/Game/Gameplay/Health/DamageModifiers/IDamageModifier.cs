using UnityEngine;

namespace CombatArena.Game.Gameplay.HealthSystem
{
    public interface IDamageModifier
    {
        public void Modify(ref Damage damage);
    }
}
