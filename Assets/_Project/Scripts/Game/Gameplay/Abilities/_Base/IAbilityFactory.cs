using System;
using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public interface IAbilityFactory<T, TConfig> where T : Ability where TConfig : AbilityConfig
    {
        T Create(TConfig config, object additionalData = null);
    }
}
