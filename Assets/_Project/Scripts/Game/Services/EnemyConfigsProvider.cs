using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class EnemyConfigsProvider
    {
        public EnemiesCollection EnemiesCollection { get; }

        public EnemyConfigsProvider()
        {
            EnemiesCollection = Resources.Load<EnemiesCollection>("Settings/EnemiesCollection");
        }
    }
}
