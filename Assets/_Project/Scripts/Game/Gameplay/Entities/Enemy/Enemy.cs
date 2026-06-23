using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.HealthSystem;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemy
{
    public class Enemy : IDisposable, IAbilityAttacker
    {
        public Health Health { get; }

        private EnemyConfig _config;
        private EnemyView _view;

        private IDisposable _damageListenerDisposable;
        private IDisposable _healthListenerDisposable;
        
        public Enemy(EnemyConfig config, EnemyView view)
        {
            _config = config;
            _view = view;

            Health = new(new DamageProcessor(), _config.MaxHealth);

            _view.UIHealth.Bind(Health);

            _healthListenerDisposable = Health.Value.Subscribe(OnHealthChange);
            _damageListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);
        }

        private void OnHealthChange(int currentValue)
        {
            if (currentValue <= 0)
            {
                // Disable Movement And All Things 
                // Death Sound
                // Death Effect
                // Death Animation
                // Dispose At Animation End

                Dispose();
            }
        }

        public void Dispose()
        {
            _view.UIHealth?.Dispose();
            _damageListenerDisposable?.Dispose();
            _healthListenerDisposable?.Dispose();
            GameObject.Destroy(_view.gameObject);
        }

        public Observable<bool> Attack(AttackAbilityConfig config)
        {
            throw new NotImplementedException();
        }

        private void TakeDamage(Damage damage)
        {
            Health.TakeDamage(damage);
        }
    }
}
