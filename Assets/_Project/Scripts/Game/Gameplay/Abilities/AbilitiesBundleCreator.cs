using System;
using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class AbilitiesBundleCreator
    {
        private IAbilitiesFactory _factory;
        private AbilitiesCollection _abilitiesCollection;

        public AbilitiesBundleCreator(IAbilitiesFactory factory, AbilitiesCollection abilitiesCollection)
        {
            _factory = factory;
            _abilitiesCollection = abilitiesCollection;
        }

        public AbilitiesBundle Create()
        {
            var abilityA = HandleAbility(_abilitiesCollection.DefaultAbilitiesBundle.AbilityA);
            var abilityX = HandleAbility(_abilitiesCollection.DefaultAbilitiesBundle.AbilityX);
            var abilityY = HandleAbility(_abilitiesCollection.DefaultAbilitiesBundle.AbilityY);

            return new AbilitiesBundle(abilityA, abilityX, abilityY);
        }

        private Ability HandleAbility<TConfig>(TConfig config) where TConfig : AbilityConfig
        {
            var configType = typeof(TConfig);

            if (configType == typeof(DashAbilityConfig))
            {
                var dashAbility = _factory.GetAbility<DashAbility, DashAbilityConfig>(config as DashAbilityConfig);

                return dashAbility;
            }

            if (configType == typeof(AttackAbilityConfig))
            {
                var attackAbility = _factory.GetAbility<AttackAbility, AttackAbilityConfig>(config as AttackAbilityConfig);

                return attackAbility;
            }

            throw new Exception($"Unsupported config: {configType}");
        }
    }
}
