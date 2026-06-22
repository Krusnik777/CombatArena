using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class DashAbility : Ability
    {
        public override AbilityConfig Config => _config;

        private DashAbilityConfig _config;

        public DashAbility(DashAbilityConfig config, object additionalData = null)
        {
            _config = config;
            SetIsReadyToUse(true);
        }

        public override void Use()
        {
            if (!IsReady()) return;
        }
    }
}
