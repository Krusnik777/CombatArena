using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.HealthSystem;
using CombatArena.Game.Services;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class Player : IDisposable, IAbilityAttacker, IAbilityMover
    {
        public Subject<Player> OnDeath { get; }

        public PlayerAbilities Abilities { get; private set; }
        public Health Health { get; }
        public Transform Transform => _view.transform; 

        private PlayerView _view;
        private GameInputService _gameInputService;
        private PlayerAvatarConfig _avatarConfig;
        private PlayerHealthConfig _healthConfig;

        private Ability _currentActiveAbility;
        private IDamageDealer _currentDamageDealer;
        private IEnemyDetector _enemyDetector;

        private CompositeDisposable _abilitiesInputListenerDisposable;

        private IDisposable _damageListenerDisposable;
        private IDisposable _healthListenerDisposable;
        private IDisposable _attackFinishListenerDisposable;

        public Player(PlayerView view, PlayerConfigsProvider configsProvider, GameInputService gameInputService)
        {
            _view = view;
            _gameInputService = gameInputService;
            _healthConfig = configsProvider.HealthConfig;
            _avatarConfig = configsProvider.AvatarConfig;

            _view.Movement.Bind(_avatarConfig, gameInputService);
            _view.Animator.Bind(_view.Movement);
            _view.Animator.SetActive(true);

            Health = new Health(new DamageProcessor(), _healthConfig.MaxHealth);
            OnDeath = new();

            _healthListenerDisposable = Health.Value.Subscribe(OnHealthChange);
            _damageListenerDisposable = _view.Damageable.OnHitted.Subscribe(TakeDamage);
        }

        public void Dispose()
        {
            _abilitiesInputListenerDisposable?.Dispose();

            _damageListenerDisposable?.Dispose();
            _healthListenerDisposable?.Dispose();
            _attackFinishListenerDisposable?.Dispose();

            Abilities?.Dispose();
            _currentDamageDealer?.Dispose();
            _enemyDetector?.Dispose();
        }

        public void AssignAbilities(PlayerAbilities abilitiesBundle)
        {
            Abilities = abilitiesBundle;
            _currentActiveAbility = null;

            _abilitiesInputListenerDisposable?.Dispose();

            _abilitiesInputListenerDisposable = new()
            {
                _gameInputService.OnAbilityAPressed.Subscribe(_ => HandleAbilityAUse()),
                _gameInputService.OnAbilityXPressed.Subscribe(_ => HandleAbilityXUse()),
                _gameInputService.OnAbilityYPressed.Subscribe(_ => HandleAbilityYUse())
            };
        }

        public IEnemyDetector EnableEnemyDetector()
        {
            _enemyDetector?.Dispose();

            _enemyDetector = new EnemyDetector(Root.LayerMasks.Enemy, Transform, _avatarConfig.EnemyDetectionRange);

            return _enemyDetector;
        }

        public void Stop()
        {
            _abilitiesInputListenerDisposable?.Dispose();
            _attackFinishListenerDisposable?.Dispose();
            _enemyDetector?.Dispose();

            _view.Movement.SetActive(false);
            _view.Animator.SetActive(false);
        }

        public Observable<bool> Attack(AttackAbilityConfig config)
        {
            var finishedAttack = new Subject<bool>();

            _attackFinishListenerDisposable?.Dispose();
            _currentDamageDealer?.Dispose();

            _view.Movement.SetActive(false);
            _attackFinishListenerDisposable = _view.EventsCollector.OnAttackFinish.Subscribe(_ =>
            {
                _attackFinishListenerDisposable?.Dispose();
                _currentDamageDealer?.Dispose();

                _view.Movement.SetActive(true);
                Health.SetIgnoreDamage(false);

                _currentActiveAbility = null;
                _currentDamageDealer = null;

                finishedAttack?.OnNext(true);
            });

            if (config.AttackType == AttackType.BasicAttack)
            {
                _currentDamageDealer = new SwordDamageDealer(Root.LayerMasks.Enemy, _view.Movement.LookTransform, config, _view.EventsCollector, _view.SwordTransform);

                _view.Animator.PlaySimpleAttack();
            }

            if (config.AttackType == AttackType.AreaAttack)
            {
                _currentDamageDealer = new AOEDamageDealer(Root.LayerMasks.Enemy, _view.transform, config, _view.EventsCollector);

                Health.SetIgnoreDamage(true);

                _view.Animator.PlaySuperAttack();
            }

            return finishedAttack;
        }

        public Observable<bool> Dash(DashAbilityConfig config)
        {
            var finishedDash = new Subject<bool>();

            Health.SetIgnoreDamage(true);
            // Show Dash Effect
            // Play Dash Sound

            _view.Movement.PerformDash(config, () =>
            {
                Health.SetIgnoreDamage(false);
                // Stop Dash Effect

                _currentActiveAbility = null;

                finishedDash?.OnNext(true);
            });

            return finishedDash;
        }

        private void HandleAbilityAUse()
        {
            if (_currentActiveAbility != null) return;

            if (Abilities.AbilityA.TryUse()) _currentActiveAbility = Abilities.AbilityA;
        }

        private void HandleAbilityXUse()
        {
            if (_currentActiveAbility != null) return;

            if (Abilities.AbilityX.TryUse()) _currentActiveAbility = Abilities.AbilityX;
        }

        private void HandleAbilityYUse()
        {
            if (_currentActiveAbility != null) return;

            if (Abilities.AbilityY.TryUse()) _currentActiveAbility = Abilities.AbilityY;
        }

        private void OnHealthChange(int currentValue)
        {
            if (currentValue <= 0)
            {
                Stop();
                _view.Animator.PlayDeath();
                // Death Sound
                // Death Effect
                // Dispose Only At Animation End

                OnDeath?.OnNext(this);
            }
        }

        private void TakeDamage(Damage damage)
        {
            bool isBlocked = UnityEngine.Random.value >= 1 - _healthConfig.ArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(_healthConfig.Armor));

            Health.TakeDamage(damage);
        }
    }
}
