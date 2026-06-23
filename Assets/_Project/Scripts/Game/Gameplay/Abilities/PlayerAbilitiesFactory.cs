using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.Entities.Player;

namespace CombatArena.Game.Gameplay
{
    public class PlayerAbilitiesFactory
    {
        private AbilitiesCollection _abilitiesCollection;

        public PlayerAbilitiesFactory(AbilitiesCollection abilitiesCollection)
        {
            _abilitiesCollection = abilitiesCollection;
        }

        public PlayerAbilities Create(Player player)
        {
            var abilityA = GetAbilityByConfig(_abilitiesCollection.DefaultPlayerAbilities.AbilityA, player);
            var abilityX = GetAbilityByConfig(_abilitiesCollection.DefaultPlayerAbilities.AbilityX, player);
            var abilityY = GetAbilityByConfig(_abilitiesCollection.DefaultPlayerAbilities.AbilityY, player);

            return new PlayerAbilities(abilityA, abilityX, abilityY);
        }

        private Ability GetAbilityByConfig(AbilityConfig config, object additionalData = null)
        {
            switch(config.Type)
            {
                case AbilityType.Dash:
                return new DashAbility(config as DashAbilityConfig, additionalData);

                case AbilityType.Attack:
                return new AttackAbility(config as AttackAbilityConfig, additionalData);

                default:
                throw new Exception($"Unsupported config: {config.Type}");
            }
        }
    }
}
