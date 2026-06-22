using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class AbilityConfigsProvider
    {
        public AbilitiesCollection AbilitiesCollection { get; }

        public AbilityConfigsProvider()
        {
            AbilitiesCollection = Resources.Load<AbilitiesCollection>("Settings/AbilitiesCollection");
        }
    }
}
