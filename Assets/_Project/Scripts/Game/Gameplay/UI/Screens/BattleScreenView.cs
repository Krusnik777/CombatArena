using UI.Windows;
using UnityEngine;

namespace CombatArena.Game.Gameplay.UI
{
    public class BattleScreenView : WindowView
    {
        [field: SerializeField] public UIHealth UIPlayerHealth { get; private set; }
        [field: SerializeField] public UIAbility UIAbilityA { get; private set; }
        [field: SerializeField] public UIAbility UIAbilityX { get; private set; }
        [field: SerializeField] public UIAbility UIAbilityY { get; private set; }
    }
}
