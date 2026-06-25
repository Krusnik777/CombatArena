using TMPro;
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
        [field: SerializeField] public TMP_Text EnemiesRemainingText { get; private set; }
        [field: SerializeField] public GameObject EnemyInfo { get; private set; }
        [field: SerializeField] public TMP_Text EnemyName { get; private set; }
        [field: SerializeField] public UIHealth UIEnemyHealth { get; private set; }
    }
}
