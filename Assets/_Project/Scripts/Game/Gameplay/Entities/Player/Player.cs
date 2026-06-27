using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.HealthSystem;
using CombatArena.Game.Services;
using DI;
using R3;
using UnityEngine;
using PlayerSounds = CombatArena.Game.Root.Sounds.PlayerSounds;

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
        private SoundService _sounds;
        private PlayerAvatarConfig _avatarConfig;
        private PlayerHealthConfig _healthConfig;

        private Ability _currentActiveAbility;
        private IDamageDealer _currentDamageDealer;
        private IEnemyDetector _enemyDetector;
        private HealthChangeVisualController _healthChangeVisualController;

        private IDisposable _attackFinishListenerDisposable;
        private IDisposable _equipListenerDisposable;

        private CompositeDisposable _abilitiesInputListenerDisposables;
        private CompositeDisposable _changesListenerDisposables;

        public Player(PlayerView view, DIContainer sceneContainer)
        {
            _view = view;
            _gameInputService = sceneContainer.Resolve<GameInputService>();
            _sounds = sceneContainer.Resolve<AudioService>().Sounds;
            var configsProvider = sceneContainer.Resolve<PlayerConfigsProvider>();
            _healthConfig = configsProvider.HealthConfig;
            _avatarConfig = configsProvider.AvatarConfig;

            _view.Movement.Bind(_avatarConfig, _gameInputService);
            _view.Movement.SetActive(false);

            _view.ShealteredSwordTransform.gameObject.SetActive(true);
            _view.SwordTransform.gameObject.SetActive(false);

            _view.Animator.SetAsCalm();
            _view.Animator.Bind(_view.Movement);
            _view.Animator.SetMovementAnimationActive(false);

            Health = new Health(new DamageProcessor(), _healthConfig.MaxHealth);
            OnDeath = new();

            _healthChangeVisualController = new(sceneContainer.Resolve<SimpleGameObjectsPool>(Root.GameplayTags.ParticlesPool), Health, _view.Particles);

            _changesListenerDisposables = new()
            {
                Health.Value.Subscribe(OnHealthChange),
                _view.Damageable.OnHitted.Subscribe(TakeDamage),
                _view.EventsCollector.OnFootstep.Subscribe(OnStep)
            };
        }

        public void Dispose()
        {
            _abilitiesInputListenerDisposables?.Dispose();

            _changesListenerDisposables?.Dispose();
            _attackFinishListenerDisposable?.Dispose();

            Abilities?.Dispose();
            _currentDamageDealer?.Dispose();
            _enemyDetector?.Dispose();
            _healthChangeVisualController?.Dispose();
            _view.Particles.Dispose();
        }

        public void SetActive(bool state)
        {
            _view.Movement.SetActive(state);
            _view.Animator.SetMovementAnimationActive(state);
        }

        public void ActivateBattleState(PlayerAbilities abilitiesBundle)
        {
            Abilities = abilitiesBundle;
            _currentActiveAbility = null;

            _abilitiesInputListenerDisposables?.Dispose();
            _equipListenerDisposable?.Dispose();

            _equipListenerDisposable = _view.EventsCollector.OnEquipWeapon.Subscribe(_ =>
            {
                _equipListenerDisposable?.Dispose();

                _view.ShealteredSwordTransform.gameObject.SetActive(false);
                _view.SwordTransform.gameObject.SetActive(true);
            });

            SetActive(false);

            _view.Animator.PlayEquip(() =>
            {
                SetActive(true);

                _abilitiesInputListenerDisposables = new()
                {
                    _gameInputService.OnAbilityAPressed.Subscribe(_ => HandleAbilityAUse()),
                    _gameInputService.OnAbilityXPressed.Subscribe(_ => HandleAbilityXUse()),
                    _gameInputService.OnAbilityYPressed.Subscribe(_ => HandleAbilityYUse())
                };
            });
        }

        public IEnemyDetector EnableEnemyDetector()
        {
            _enemyDetector?.Dispose();

            _enemyDetector = new EnemyDetector(Root.LayerMasks.Enemy, Transform, _avatarConfig.EnemyDetectionRange);

            return _enemyDetector;
        }

        public void Stop()
        {
            _abilitiesInputListenerDisposables?.Dispose();
            _attackFinishListenerDisposable?.Dispose();
            _enemyDetector?.Dispose();
            _healthChangeVisualController?.ClearHitNumbers();
            _healthChangeVisualController?.Dispose();

            _view.Movement.SetActive(false);
            _view.Animator.SetMovementAnimationActive(false);
            _view.Particles.Dispose();
        }

        public void ActivateVictoryState()
        {
            _equipListenerDisposable?.Dispose();

            _equipListenerDisposable = _view.EventsCollector.OnDisarmWeapon.Subscribe(_ =>
            {
                _equipListenerDisposable?.Dispose();

                _view.ShealteredSwordTransform.gameObject.SetActive(true);
                _view.SwordTransform.gameObject.SetActive(false);
            });

            _view.Animator.PlayWin();
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
                _currentDamageDealer.SubscribeToAttack(() => _sounds.Play(PlayerSounds.Attack));

                _view.Animator.PlaySimpleAttack();
            }

            if (config.AttackType == AttackType.AreaAttack)
            {
                _currentDamageDealer = new AOEDamageDealer(Root.LayerMasks.Enemy, _view.transform, config, _view.EventsCollector);
                _currentDamageDealer.SubscribeToAttack(() => _sounds.Play(PlayerSounds.AreaAttack));

                Health.SetIgnoreDamage(true);

                _view.Animator.PlaySuperAttack();
            }

            return finishedAttack;
        }

        public Observable<bool> Dash(DashAbilityConfig config)
        {
            var finishedDash = new Subject<bool>();

            Health.SetIgnoreDamage(true);
            _view.DashEffects.Show();
            _sounds.Play(PlayerSounds.Dash);

            _view.Movement.PerformDash(config, () =>
            {
                Health.SetIgnoreDamage(false);
                _view.DashEffects.Hide();

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
                _sounds.Play(PlayerSounds.Death);

                OnDeath?.OnNext(this);
            }
        }

        private void TakeDamage(Damage damage)
        {
            bool isBlocked = UnityEngine.Random.value >= 1 - _healthConfig.ArmorDefenceChance;
            if (isBlocked) damage.Modifiers.Add(new ArmorDefenceModifier(_healthConfig.Armor));

            Health.TakeDamage(damage);

            if (!Health.IsDamageIgnored) _sounds.Play(PlayerSounds.Damage);
        }

        private void OnStep(int legIndex)
        {
            int stepIndex = UnityEngine.Random.Range(0, PlayerSounds.Steps.Length);
            _sounds.Play(PlayerSounds.Steps[stepIndex]);
        }
    }
}
