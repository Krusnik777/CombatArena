using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.HealthSystem;
using CombatArena.Game.Services;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class Enemy : IDisposable, IAbilityAttacker
    {
        public Subject<Enemy> OnDeath { get; }
        public Subject<Enemy> OnDeleted { get; }

        public Health Health { get; }
        public string Name => _config.Name;

        public bool IsSameView(EnemyView enemyView) => enemyView == _view;

        private EnemyConfig _config;
        private EnemyView _view;
        private SoundService _sounds;

        private AttackAbility _attackAbility;
        private TargetPursuer _currentPursuer;
        private IDamageDealer _currentDamageDealer;
        private HealthChangeVisualController _healthChangeVisualController;

        private IDisposable _damageListenerDisposable;
        private IDisposable _healthListenerDisposable;
        private IDisposable _attackFinishListenerDisposable;
        private IDisposable _deathDisposable;

        public Enemy(EnemyConfig config, EnemyView view, SoundService sounds)
        {
            _config = config;
            _view = view;
            _sounds = sounds;

            _attackAbility = new AttackAbility(config.AttackAbility, this);

            Health = new(new DamageProcessor(), _config.MaxHealth);
            OnDeath = new();
            OnDeleted = new();

            _view.UIHealth.Bind(Health);

            _healthListenerDisposable = Health.Value.Subscribe(OnHealthChange);
            _damageListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);
        }

        public void Dispose()
        {
            Stop();
        }

        public void Disable()
        {
            _view.Agent.enabled = false;
            _view.gameObject.SetActive(false);
        }

        public void ActivateAndAssignPurseTarget(Transform target)
        {
            _view.gameObject.SetActive(true);
            _view.Agent.enabled = true;

            AssignPursueTarget(target);
        }

        public void AssignPursueTarget(Transform target)
        {
            _currentPursuer?.Dispose();

            _currentPursuer = new(target, _view.Agent, _config.MovementSpeed, _config.AttackAbility.Range);
            _currentPursuer.SetActionOnReach(TryToAttack);

            _currentPursuer.StartPursue();
        }

        public void ActivateParticles(SimpleGameObjectsPool particlesPool)
        {
            if (_view.Particles == null) return;

            _healthChangeVisualController?.Dispose();

            _healthChangeVisualController = new(particlesPool, Health, _view.Particles);
        }

        public void CleanupHitNumbers() => _healthChangeVisualController?.ClearHitNumbers();

        public void Stop()
        {
            _deathDisposable?.Dispose();

            _view.Damageable.gameObject.SetActive(false);
            _view.ChosenEffect.SetActive(false);
            _view.Agent.enabled = false;
            _view.Particles.Dispose();

            _view.UIHealth.gameObject.SetActive(false);
            _view.UIHealth?.Dispose();

            _damageListenerDisposable?.Dispose();
            _healthListenerDisposable?.Dispose();
            _attackFinishListenerDisposable?.Dispose();

            _attackAbility?.Dispose();
            _currentPursuer?.Dispose();
            _currentDamageDealer?.Dispose();
            _healthChangeVisualController?.Dispose();
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
            _currentDamageDealer.SubscribeToAttack(() => _sounds.Play(_config.AttackSound));

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
                Stop();
                _view.Animator.PlayDeath();
                _sounds.Play(_config.DeathSound);

                _deathDisposable = Observable.Interval(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
                {
                    OnDeleted?.OnNext(this);
                    _view.gameObject.SetActive(false);
                    Dispose();
                });

                OnDeath?.OnNext(this);
            }
        }

        private void TakeDamage(Damage damage)
        {
            bool isBlocked = UnityEngine.Random.value >= 1 - _config.ArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(_config.Armor));

            Health.TakeDamage(damage);

            _sounds.Play(_config.DamageSound);
        }
    }
}
