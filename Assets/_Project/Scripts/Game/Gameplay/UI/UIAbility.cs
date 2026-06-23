using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIAbility : MonoBehaviour, IDisposable
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private Image m_fillReadyImage;

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
                ability.OnUsed.Subscribe(_ => m_fillReadyImage.fillAmount = 1f),
                ability.OnExecuted.Subscribe(_ => StartListenAbilityCooldown()),
                ability.OnCooldownCompleted.Subscribe(_ => StopListenAbilityCooldown())
            };
        }

        public void Dispose()
        {
            DisposeOfListeners();
        }

        private void DisposeOfListeners()
        {
            _abilityActionsListenerDisposable?.Dispose();
            _cooldownListenerDisposable?.Dispose();
        }

        private void StartListenAbilityCooldown()
        {
            _cooldownListenerDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
               m_fillReadyImage.fillAmount = 1f - _bindedAbility.CurrentCooldownRate; 
            });
        }

        private void StopListenAbilityCooldown()
        {
            _cooldownListenerDisposable?.Dispose();

            m_fillReadyImage.fillAmount = 0f;
        }
    }
}
