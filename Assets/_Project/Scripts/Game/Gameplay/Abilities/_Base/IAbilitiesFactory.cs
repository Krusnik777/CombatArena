using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public interface IAbilitiesFactory
    {
        public T GetAbility<T,TConfig>(TConfig config, object additionalData = null) where T : Ability where TConfig : AbilityConfig;
    }
}