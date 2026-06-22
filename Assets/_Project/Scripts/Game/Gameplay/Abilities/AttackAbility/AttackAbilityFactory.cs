using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class AttackAbilityFactory : IAbilityFactory<AttackAbility, AttackAbilityConfig>
    {
        public AttackAbility Create(AttackAbilityConfig config, object additionalData = null)
        {
            return new AttackAbility(config, additionalData);
        }
    }
}