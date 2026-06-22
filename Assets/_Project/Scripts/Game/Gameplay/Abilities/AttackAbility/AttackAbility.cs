using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class AttackAbility : Ability
    {
        public override AbilityConfig Config => _config;

        private AttackAbilityConfig _config;

        public AttackAbility(AttackAbilityConfig config, object additionalData = null)
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
