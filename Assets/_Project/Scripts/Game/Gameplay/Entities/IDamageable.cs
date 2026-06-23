using CombatArena.Game.Gameplay.HealthSystem;

namespace CombatArena.Game.Gameplay.Entities
{
    public interface IDamageable
    {
        public void Hit(Damage damage);
    }
}
