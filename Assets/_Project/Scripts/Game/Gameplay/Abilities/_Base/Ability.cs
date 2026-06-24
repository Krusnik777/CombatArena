using System;
using CombatArena.Game.Configs;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay
{
    public abstract class Ability : IAbility, IDisposable
    {
        public readonly Subject<Ability> OnUsed = new();
        public readonly Subject<Ability> OnExecuted = new();
        public readonly Subject<Ability> OnCooldownCompleted = new();

        public abstract AbilityConfig Config { get; }
        public abstract bool TryUse();

        public float CurrentCooldownRate => Config.Cooldown == 0f ? 1f : _currentTime/Config.Cooldown;

        private bool _isReadyToUse;
        private float _currentTime;

        private IDisposable _cooldownListenerDisposable;

        public virtual void Dispose()
        {
            _cooldownListenerDisposable?.Dispose();
        }

        public virtual bool IsReady() => CurrentCooldownRate >= 1f && _isReadyToUse;
        protected virtual void SetIsReadyToUse(bool state) => _isReadyToUse = state;
        protected virtual void SetCooldownAsCompleted()
        {
            _cooldownListenerDisposable?.Dispose();

            _currentTime = Config.Cooldown;

            OnCooldownCompleted?.OnNext(this);
        }

        protected void StartCooldown()
        {
            _currentTime = 0f;

            _cooldownListenerDisposable = Observable.EveryUpdate().Subscribe(_ => UpdateCooldownTime());
        }

        private void UpdateCooldownTime()
        {
            if (CurrentCooldownRate >= 1f)
            {
                SetCooldownAsCompleted();
                return;
            }

            _currentTime += Time.deltaTime;
        }
    }
}