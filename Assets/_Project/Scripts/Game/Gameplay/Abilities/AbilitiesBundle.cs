using System;

namespace CombatArena.Game.Gameplay
{
    public class AbilitiesBundle
    {
        public Ability AbilityA { get; }
        public Ability AbilityX { get; }
        public Ability AbilityY { get; }

        public AbilitiesBundle(Ability abilityA, Ability abilityX, Ability abilityY)
        {
            AbilityA = abilityA;
            AbilityX = abilityX;
            AbilityY = abilityY;
        }
    }
}