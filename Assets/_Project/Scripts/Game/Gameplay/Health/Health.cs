using R3;

namespace CombatArena.Game.Gameplay.HealthSystem
{
    public struct HealthChange
    {
        public enum ChangeType
        {
            Damage,
            CriticalDamage,
            Heal,
            CriticalHeal
        }

        public int Value;
        public ChangeType Type;
    }

    public class Health
    {
        public int MaxValue { get; }
        public Observable<int> Value => _currentValue;
        public Subject<HealthChange> OnChange { get; }

        private IDamageProcessor _damageProcessor;

        private ReactiveProperty<int> _currentValue;

        public Health(IDamageProcessor damageProcessor, int maxValue)
        {
            MaxValue = maxValue;
            _damageProcessor = damageProcessor;
            
            _currentValue = new(MaxValue);

            OnChange = new();
        }

        public void TakeDamage(Damage damage)
        {
            if (_currentValue.Value <= 0) return;

            _damageProcessor.Process(ref damage);

            var value = _currentValue.Value;
            value -= damage.ResultValue;

            if (value <= 0)
            {
                InvokeOnChange(_currentValue.Value, damage.IsCritical ? HealthChange.ChangeType.CriticalDamage : HealthChange.ChangeType.Damage);

                _currentValue.Value = 0;
            }
            else
            {
                InvokeOnChange(damage.ResultValue, damage.IsCritical ? HealthChange.ChangeType.CriticalDamage : HealthChange.ChangeType.Damage);

                _currentValue.Value = value;
            }
        }

        public void Heal(int healAmount)
        {
            var value = _currentValue.Value;
            value += healAmount;

            if (value >= MaxValue)
            {
                InvokeOnChange(MaxValue - _currentValue.Value, HealthChange.ChangeType.Heal);

                _currentValue.Value = MaxValue;
            }
            else
            {
                InvokeOnChange(healAmount, HealthChange.ChangeType.Heal);

                _currentValue.Value = value;
            }
        }

        private void InvokeOnChange(int value, HealthChange.ChangeType changeType)
        {
            OnChange.OnNext(new HealthChange
            {
                Value = value,
                Type = changeType
            });
        }
    }
}
