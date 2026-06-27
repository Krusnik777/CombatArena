using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class BeforeBattleScreenView : WindowView
    {
        [field: SerializeField] public RectTransform[] ScreenBorders { get; private set; }
        [field: SerializeField] public Image FadeImage { get; private set; }
        [field: SerializeField] public TMP_Text Text { get; private set; }
    }
}
