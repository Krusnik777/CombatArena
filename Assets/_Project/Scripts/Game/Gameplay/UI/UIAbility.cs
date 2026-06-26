using System;
using R3;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIAbility : TooltipTrigger, IDisposable
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private Image m_fillReadyImage;
        [SerializeField] private GameObject m_tipControl;

        protected override TooltipType _tooltipeType => TooltipType.Ability;

        private Ability _bindedAbility;

        private CompositeDisposable _abilityActionsListenerDisposable;
        private IDisposable _cooldownListenerDisposable;

        public void Bind(Ability ability)
        {
            DisposeOfListeners();

            _bindedAbility = ability;

            m_icon.sprite = ability.Config.Icon;
            m_fillReadyImage.fillAmount = 1f - ability.CurrentCooldownRate;

            _abilityActionsListenerDisposable = new()
            {
                ability.OnUsed.Subscribe(_ =>
                {
                    m_fillReadyImage.fillAmount = 1f;
                    if (m_tipControl != null) m_tipControl.SetActive(false);
                }),
                ability.OnExecuted.Subscribe(_ => StartListenAbilityCooldown()),
                ability.OnCooldownCompleted.Subscribe(_ => StopListenAbilityCooldown())
            };

            ChangeTooltipIfActive();
        }

        public void Dispose()
        {
            DisposeOfListeners();
        }

        public override TooltipData GetTooltipData()
        {
            if (_bindedAbility == null) return null;

            string infoText = "Cooldown: None";

            if (_bindedAbility.Config.Cooldown > 0)
            {
                TimeSpan time = TimeSpan.FromSeconds(_bindedAbility.Config.Cooldown);
                string mins = time.Minutes > 0 ? $" {time.Minutes} m" : "";
                string seconds = time.Seconds > 0 ? $" {time.Seconds} s" : "";

                infoText = $"Cooldown:{mins}{seconds}";
            }

            return new TooltipData(_bindedAbility.Config.Name, _bindedAbility.Config.Description, infoText);
        }

        private void DisposeOfListeners()
        {
            _abilityActionsListenerDisposable?.Dispose();
            _cooldownListenerDisposable?.Dispose();
        }

        private void StartListenAbilityCooldown()
        {
            _cooldownListenerDisposable?.Dispose();
            
            _cooldownListenerDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                m_fillReadyImage.fillAmount = 1f - _bindedAbility.CurrentCooldownRate;
            });
        }

        private void StopListenAbilityCooldown()
        {
            _cooldownListenerDisposable?.Dispose();

            m_fillReadyImage.fillAmount = 0f;
            if (m_tipControl != null) m_tipControl.SetActive(true);
        }
    }
}
