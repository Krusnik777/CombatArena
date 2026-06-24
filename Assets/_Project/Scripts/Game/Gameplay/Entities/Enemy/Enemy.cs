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

        private AttackAbility _attackAbility;
        private TargetPursuer _currentPursuer;
        private IDamageDealer _currentDamageDealer;

        private IDisposable _damageListenerDisposable;
        private IDisposable _healthListenerDisposable;
        private IDisposable _attackFinishListenerDisposable;
        
        public Enemy(EnemyConfig config, EnemyView view)
        {
            _config = config;
            _view = view;

            Health = new(new DamageProcessor(), _config.MaxHealth);
            _attackAbility = new AttackAbility(config.AttackAbility, this);

            _view.UIHealth.Bind(Health);

            _healthListenerDisposable = Health.Value.Subscribe(OnHealthChange);
            _damageListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);
        }

        public void Dispose()
        {
            _view.UIHealth?.Dispose();

            _damageListenerDisposable?.Dispose();
            _healthListenerDisposable?.Dispose();
            _attackFinishListenerDisposable?.Dispose();

            _attackAbility?.Dispose();
            _currentPursuer?.Dispose();
            _currentDamageDealer?.Dispose();

            GameObject.Destroy(_view.gameObject);
        }

        public void AssignPursueTarget(Transform target)
        {
            _currentPursuer?.Dispose();

            _currentPursuer = new(target, _view.Agent, _config.MovementSpeed, _config.AttackAbility.Range);
            _currentPursuer.SetActionOnReach(TryToAttack);

            _currentPursuer.StartPursue();
        }

        public Observable<bool> Attack(AttackAbilityConfig config)
        {
            var finishedAttack = new Subject<bool>();

            _attackFinishListenerDisposable?.Dispose();
            _currentDamageDealer?.Dispose();

            _currentPursuer?.StopPursue();

            _attackFinishListenerDisposable = _view.EventsCollector.OnAttackFinish.Subscribe(_ =>
            {
                _attackFinishListenerDisposable?.Dispose();
                _currentDamageDealer?.Dispose();

                _currentPursuer?.StartPursue();

                _currentDamageDealer = null;

                finishedAttack?.OnNext(true);
            });

            _currentDamageDealer = new AOEDamageDealer(Root.LayerMasks.Player, _view.transform, config, _view.EventsCollector);
            _view.Animator.PlayAttack();

            return finishedAttack;
        }

        private void TryToAttack()
        {
            _attackAbility?.TryUse();
        }

        private void OnHealthChange(int currentValue)
        {
            if (currentValue <= 0)
            {
                // Disable Movement And All Things 
                // Death Sound
                // Death Effect
                // Death Animation
                // Dispose Only At Animation End

                Dispose();
            }
        }

        private void TakeDamage(Damage damage)
        {
            bool isBlocked = UnityEngine.Random.value >= 1 - _config.ArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(_config.Armor));

            Health.TakeDamage(damage);
        }
    }
}
