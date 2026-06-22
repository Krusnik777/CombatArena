using System;
using CombatArena.Game.Configs;
using CombatArena.Game.Services;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class Player : IDisposable, IAbilityAttacker, IAbilityMover
    {
        private PlayerView _view;
        private AbilitiesBundle _abilities;
        // also Health or something

        private Ability _currentActiveAbility;

        private CompositeDisposable _testDisposable;

        public Player(PlayerView view, PlayerAvatarConfig config, GameInputService gameInputService, AbilitiesBundle abilitiesBundle)
        {
            _view = view;
            _view.Movement.Bind(config, gameInputService);

            _abilities = abilitiesBundle;
            _currentActiveAbility = null;

            _testDisposable = new()
            {
                gameInputService.OnAbilityAPressed.Subscribe(_ => HandleAbilityA()),
                gameInputService.OnAbilityXPressed.Subscribe(_ => HandleAbilityX()),
                gameInputService.OnAbilityYPressed.Subscribe(_ => HandleAbilityY())
            };
        }

        public void Dispose()
        {
            _testDisposable?.Dispose();
        }

        public Observable<bool> Attack(AttackAbilityConfig config)
        {
            if (config.AttackType == AttackType.HorizontalSlash) _view.Animator.PlaySimpleAttack();
            else if (config.AttackType == AttackType.JumpAttack) _view.Animator.PlaySuperAttack();

            throw new NotImplementedException();
        }

        public Observable<bool> Dash(DashAbilityConfig config)
        {
            _view.Movement.PerformDash(config);

            throw new NotImplementedException();
        }

        private void HandleAbilityA()
        {
            if (_currentActiveAbility != null) return;

            _abilities.AbilityA.Use();
        }

        private void HandleAbilityX()
        {
            if (_currentActiveAbility != null) return;

            _abilities.AbilityX.Use();
        }

        private void HandleAbilityY()
        {
            if (_currentActiveAbility != null) return;

            _abilities.AbilityY.Use();
        }
    }
}
