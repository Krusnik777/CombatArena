using System;
using CombatArena.Game.Gameplay.HealthSystem;
using UnityEngine;
using UnityEngine.UI;
using R3;
using UI.Tooltips;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIHealth : TooltipTrigger, IDisposable
    {
        [SerializeField] private Image m_fillImage;

        protected override TooltipType _tooltipeType => TooltipType.Health;

        private Health _bindedHealth;

        private IDisposable _healthValueChangeListenerDisposable;

        public void Bind(Health health)
        {
            DisposeOfListeners();

            _bindedHealth = health;

            m_fillImage.fillAmount = 1;

            _healthValueChangeListenerDisposable = _bindedHealth.Value.Subscribe(OnHealthChange);

            ChangeTooltipIfActive();

            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            DisposeOfListeners();
        }

        public override TooltipData GetTooltipData()
        {
            if (_bindedHealth == null) return null;

            return new TooltipData("Health", _bindedHealth.HealthStatus);
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
