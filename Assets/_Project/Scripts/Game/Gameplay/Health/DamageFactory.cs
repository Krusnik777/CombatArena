using CombatArena.Game.Configs;
using Random = UnityEngine.Random;

namespace CombatArena.Game.Gameplay.HealthSystem
{
    public static class DamageFactory
    {
        public static Damage Create(AttackAbilityConfig config)
        {
            bool isCritical = Random.value >= 1 - config.CriticalChance;
            bool isArmorBreak = Random.value >= 1 - config.ArmorBreakChance;

            var damage = new Damage()
            {
                BaseValue = config.Damage,
                ResultValue = config.Damage,
                Modifiers = new()
            };

            if (isCritical) damage.Modifiers.Add(new CriticalDamageModifier());
            if (isArmorBreak) damage.Modifiers.Add(new ArmorBreakDamageModifier());

            return damage;
        }
    }
}
