using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class DamageableMasksLoader
    {
        public void LoadMasksAndAssignToLayerMasks()
        {
            var config = Resources.Load<DamageableMasksConfig>("Settings/DamageableMasksConfig");

            Root.LayerMasks.Player = config.Player;
            Root.LayerMasks.Enemy = config.Enemy;
        }
    }
}
