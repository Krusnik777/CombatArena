using System;
using System.Collections.Generic;
using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public class AbilitiesFactory : IAbilitiesFactory
    {
        private readonly Dictionary<Type, object> _factories;

        public AbilitiesFactory()
        {
            _factories = new();

            RegisterFactory(new DashAbilityFactory());
            RegisterFactory(new AttackAbilityFactory());
        }

        public T GetAbility<T,TConfig>(TConfig config, object additionalData = null) where T : Ability where TConfig : AbilityConfig
        {
            Type t = typeof(T);
            Type configType = typeof(TConfig);

            if (_factories.TryGetValue(t, out var findedFactory) && findedFactory is IAbilityFactory<T,TConfig> factory)
            {
                return factory.Create(config, additionalData);
            }

            throw new Exception($"Unsupported ability or ability-config pair: config - {configType}, type = {t}");
        }

        private void RegisterFactory<T, TConfig>(IAbilityFactory<T, TConfig> factory) where T : Ability where TConfig : AbilityConfig
        {
            _factories[typeof(T)] = factory;
        }
    }
}
