using CombatArena.Game.Configs;
using CombatArena.Game.Gameplay.Entities;
using R3;

namespace CombatArena.Game.Gameplay
{
    public class DashAbility : Ability
    {
        public override AbilityConfig Config => _config;

        private DashAbilityConfig _config;
        private IAbilityMover _mover;

        private System.IDisposable _dashExecutionListenerDisposable;

        public DashAbility(DashAbilityConfig config, object additionalData = null)
        {
            if (additionalData == null || additionalData is not IAbilityMover) throw new System.NullReferenceException("Need to have additionalData as IAbilityMover");

            _config = config;
            _mover = additionalData as IAbilityMover;
            SetIsReadyToUse(true);
            SetCooldownAsCompleted();
        }

        public override void Dispose()
        {
            base.Dispose();

            _dashExecutionListenerDisposable?.Dispose();
        }

        public override bool TryUse()
        {
            if (!IsReady()) return false;

            SetIsReadyToUse(false);
            OnUsed?.OnNext(this);

            _dashExecutionListenerDisposable = _mover.Dash(_config).Subscribe(_ =>
            {
                _dashExecutionListenerDisposable?.Dispose();

                StartCooldown();
                SetIsReadyToUse(true);

                OnExecuted?.OnNext(this);
            });

            return true;
        }
    }
}
