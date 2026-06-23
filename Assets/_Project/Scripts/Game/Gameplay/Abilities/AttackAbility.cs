using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.Entities;
using R3;

namespace CombatArena.Game.Gameplay
{
    public class AttackAbility : Ability
    {
        public override AbilityConfig Config => _config;

        private AttackAbilityConfig _config;
        private IAbilityAttacker _attacker;

        private System.IDisposable _attackExecutionListenerDisposable;

        public AttackAbility(AttackAbilityConfig config, object additionalData = null)
        {
            if (additionalData == null || additionalData is not IAbilityAttacker) throw new System.NullReferenceException("Need to have additionalData as IAbilityAttacker");

            _config = config;
            _attacker = additionalData as IAbilityAttacker;
            SetIsReadyToUse(true);
            SetCooldownAsCompleted();
        }
        
        public override void Dispose()
        {
            base.Dispose();

            _attackExecutionListenerDisposable?.Dispose();
        }

        public override bool TryUse()
        {
            if (!IsReady()) return false;

            SetIsReadyToUse(false);
            OnUsed?.OnNext(this);

            _attackExecutionListenerDisposable = _attacker.Attack(_config).Subscribe(_ =>
            {
                _attackExecutionListenerDisposable?.Dispose();

                StartCooldown();
                SetIsReadyToUse(true);

                OnExecuted?.OnNext(this);
            });

            return true;
        }
    }
}
