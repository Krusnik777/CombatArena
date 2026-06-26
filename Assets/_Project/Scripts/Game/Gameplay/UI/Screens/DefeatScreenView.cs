using R3;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class DefeatScreenView : WindowView
    {
        [field: SerializeField] public UIControlsTip[] ControlsTips { get; private set; }
        [SerializeField] private Button m_restartButton;
        [SerializeField] private Button m_giveUpButton;

        public Subject<Unit> OnRestartPress { get; private set; } = new();
        public Subject<Unit> OnGiveUpPress { get; private set; } = new();

        private void OnEnable()
        {
            m_restartButton.onClick.AddListener(OnRestartButtonPressed);
            m_giveUpButton.onClick.AddListener(OnGiveUpButtonPressed);
        }

        private void OnDisable()
        {
            m_restartButton.onClick.RemoveListener(OnRestartButtonPressed);
            m_giveUpButton.onClick.RemoveListener(OnGiveUpButtonPressed);
        }

        private void OnRestartButtonPressed() => OnRestartPress?.OnNext(Unit.Default);
        private void OnGiveUpButtonPressed() => OnGiveUpPress?.OnNext(Unit.Default);
    }
}
