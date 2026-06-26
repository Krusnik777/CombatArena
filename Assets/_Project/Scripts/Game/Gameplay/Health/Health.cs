using R3;

namespace CombatArena.Game.Gameplay.HealthSystem
{
    public class Health
    {
        public int MaxValue { get; }
        public Observable<int> Value => _currentValue;
        public Subject<Damage> OnDamage { get; } 
        public Subject<int> OnHeal { get; } 

        public string HealthStatus => $"{_currentValue.Value}/{MaxValue}";

        private IDamageProcessor _damageProcessor;

        private ReactiveProperty<int> _currentValue;

        private bool _ignoreDamage;

        public Health(IDamageProcessor damageProcessor, int maxValue)
        {
            MaxValue = maxValue;
            _damageProcessor = damageProcessor;
            
            _currentValue = new(MaxValue);

            OnDamage = new();
            OnHeal = new();
        }

        public void SetIgnoreDamage(bool state) => _ignoreDamage = state;

        public void TakeDamage(Damage damage)
        {
            if (_currentValue.Value <= 0) return;

            if (_ignoreDamage) return;

            _damageProcessor.Process(ref damage);

            var value = _currentValue.Value;
            value -= damage.ResultValue;

            OnDamage?.OnNext(damage);

            if (value <= 0)
            {
                _currentValue.Value = 0;
            }
            else
            {
                _currentValue.Value = value;
            }
        }

        public void Heal(int healAmount)
        {
            var value = _currentValue.Value;
            value += healAmount;

            if (value >= MaxValue)
            {
                OnHeal?.OnNext(MaxValue - _currentValue.Value);

                _currentValue.Value = MaxValue;
            }
            else
            {
                OnHeal?.OnNext(healAmount);

                _currentValue.Value = value;
            } 
        }
    }
}
