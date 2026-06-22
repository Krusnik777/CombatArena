using CombatArena.Game.Configs;
using R3;

namespace CombatArena.Game.Gameplay.Entities
{
    public interface IAbilityAttacker
    {
        public Observable<bool> Attack(AttackAbilityConfig config);
    }
}
