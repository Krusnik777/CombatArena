using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.HealthSystem;
using CombatArena.Game.Services;
using R3;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class Player : IDisposable, IAbilityAttacker, IAbilityMover
    {
        public PlayerAbilities Abilities { get; private set; }
        public Health Health { get; }

        private PlayerView _view;
        private GameInputService _gameInputService;
        private PlayerHealthConfig _healthConfig;

        private Ability _currentActiveAbility;
        private DamageDealer _currentDamageDealer;

        private CompositeDisposable _abilitiesInputListenerDisposable;
        private CompositeDisposable _attackListenerDisposable;

        public Player(PlayerView view, PlayerConfigsProvider configsProvider, GameInputService gameInputService)
        {
            _view = view;
            _gameInputService = gameInputService;
            _healthConfig = configsProvider.HealthConfig;

            _view.Movement.Bind(configsProvider.AvatarConfig, gameInputService);
            _view.Animator.Bind(_view.Movement);

            Health = new Health(new DamageProcessor(), _healthConfig.MaxHealth);
        }

        public void Dispose()
        {
            _abilitiesInputListenerDisposable?.Dispose();
            _attackListenerDisposable?.Dispose();
            Abilities?.Dispose();
            _currentDamageDealer?.Dispose();
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

        public Observable<bool> Attack(AttackAbilityConfig config)
        {
            var finishedAttack = new Subject<bool>();

            _attackListenerDisposable?.Dispose();
            _currentDamageDealer?.Dispose();
            _attackListenerDisposable = new();

            _view.Movement.SetActive(false);
            _attackListenerDisposable.Add(_view.EventsCollector.OnAttackFinish.Subscribe(_ =>
            {
                _attackListenerDisposable?.Dispose();
                _currentDamageDealer?.Dispose();

                _view.Movement.SetActive(true);

                _currentActiveAbility = null;
                _currentDamageDealer = null;

                finishedAttack?.OnNext(true);
            }));

            _currentDamageDealer = new DamageDealer(_view.transform, config, _view.EventsCollector);

            if (config.AttackType == AttackType.HorizontalSlash)
            {
                // Create Damage Handler which workd from start to finish

                _view.Animator.PlaySimpleAttack();
            }

            if (config.AttackType == AttackType.JumpAttack)
            {
                // Create Damage Handler which look for OnExecuted

                _view.Animator.PlaySuperAttack();
            }

            return finishedAttack;
        }

        public Observable<bool> Dash(DashAbilityConfig config)
        {
            var finishedDash = new Subject<bool>();

            // Enable Invulnerability
            // Show Dash Effect
            // Play Dash Sound

            _view.Movement.PerformDash(config, () =>
            {
                // Disable Invulnerability
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
    }
}
