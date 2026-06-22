using CombatArena.Game.Configs;
using R3;

namespace CombatArena.Game.Gameplay.Entities
{
    public interface IAbilityMover
    {
        public Observable<bool> Dash(DashAbilityConfig config);
    }
}
