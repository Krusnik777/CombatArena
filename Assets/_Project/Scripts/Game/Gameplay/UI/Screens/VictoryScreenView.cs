using R3;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class VictoryScreenView : WindowView
    {
        [SerializeField] private Button m_againButton;
        [SerializeField] private Button m_exitButton;

        public Subject<Unit> OnAgainPress { get; private set; } = new();
        public Subject<Unit> OnExitPress { get; private set; } = new();

        private void OnEnable()
        {
            m_againButton.onClick.AddListener(OnAgainButtonPressed);
            m_exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        private void OnDisable()
        {
            m_againButton.onClick.RemoveListener(OnAgainButtonPressed);
            m_exitButton.onClick.RemoveListener(OnExitButtonPressed);
        }

        private void OnAgainButtonPressed() => OnAgainPress?.OnNext(Unit.Default);
        private void OnExitButtonPressed() => OnExitPress?.OnNext(Unit.Default);
    }
}
