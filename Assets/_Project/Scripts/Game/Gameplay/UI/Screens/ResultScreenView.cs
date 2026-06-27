using R3;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public abstract class ResultScreenView : WindowView
    {
        [field: SerializeField] public TMP_Text Title { get; private set; }
        [field: SerializeField] public TMP_Text Message { get; private set; }
        [field: SerializeField] public GameObject ButtonsContainer { get; private set;}
        [field: SerializeField] public UIControlsTip[] ControlsTips { get; private set; }
        [SerializeField] private Button m_acceptButton;
        [SerializeField] private Button m_declineButton;

        public Subject<Unit> OnAcceptPress { get; private set; } = new();
        public Subject<Unit> OnDeclinePress { get; private set; } = new();

        protected virtual void OnEnable()
        {
            m_acceptButton.onClick.AddListener(OnAcceptButtonPressed);
            m_declineButton.onClick.AddListener(OnDeclineButtonPressed);
        }

        protected virtual void OnDisable()
        {
            m_acceptButton.onClick.RemoveListener(OnAcceptButtonPressed);
            m_declineButton.onClick.RemoveListener(OnDeclineButtonPressed);
        }

        private void OnAcceptButtonPressed() => OnAcceptPress?.OnNext(Unit.Default);
        private void OnDeclineButtonPressed() => OnDeclinePress?.OnNext(Unit.Default);
    }
}
