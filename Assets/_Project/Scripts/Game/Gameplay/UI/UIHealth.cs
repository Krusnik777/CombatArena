using System;
using CombatArena.Game.Gameplay.HealthSystem;
using UnityEngine;
using UnityEngine.UI;
using R3;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIHealth : MonoBehaviour, IDisposable
    {
        [SerializeField] private Image m_fillImage;

        private Health _bindedHealth;

        private IDisposable _healthValueChangeListenerDisposable;

        public void Bind(Health health)
        {
            DisposeOfListeners();

            _bindedHealth = health;

            m_fillImage.fillAmount = 1;

            _healthValueChangeListenerDisposable = _bindedHealth.Value.Subscribe(OnHealthChange);
        }

        public void Dispose()
        {
            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {
            _healthValueChangeListenerDisposable?.Dispose();
        }

        private void OnHealthChange(int currentValue)
        {
            m_fillImage.fillAmount = (float)currentValue / (float)_bindedHealth.MaxValue;
        }
    }
}
