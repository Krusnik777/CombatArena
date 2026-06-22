using System;
using CombatArena.Game.Configs;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay
{
    public abstract class Ability : IAbility, IDisposable
    {
        public abstract AbilityConfig Config { get; }
        public abstract void Use();

        public float CurrentCooldownRate => _currentTime/Config.Cooldown;

        private bool _isReadyToUse;
        private float _currentTime;

        private IDisposable _cooldownListenerDisposable;

        public virtual void Dispose()
        {
            _cooldownListenerDisposable?.Dispose();
        }

        public virtual bool IsReady() => CurrentCooldownRate >= 1f && _isReadyToUse;
        protected virtual void SetIsReadyToUse(bool state) => _isReadyToUse = state;

        protected void StartCooldown()
        {
            _currentTime = 0f;

            _cooldownListenerDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateCooldownTime());
        }

        private void UpdateCooldownTime()
        {
            if (CurrentCooldownRate >= 1f)
            {
                _cooldownListenerDisposable?.Dispose();

                return;
            }

            _currentTime += Time.deltaTime;
        }
    }
}