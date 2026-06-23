using CombatArena.Game.Configs;

namespace CombatArena.Game.Gameplay
{
    public interface IAbility
    {
        public AbilityConfig Config { get; }
        public float CurrentCooldownRate { get; }

        public bool TryUse();
        public bool IsReady();
    }
}
