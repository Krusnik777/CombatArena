using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class DashAbilityFactory : IAbilityFactory<DashAbility, DashAbilityConfig>
    {
        public DashAbility Create(DashAbilityConfig config, object additionalData = null)
        {
            return new DashAbility(config, additionalData);
        }
    }
}
