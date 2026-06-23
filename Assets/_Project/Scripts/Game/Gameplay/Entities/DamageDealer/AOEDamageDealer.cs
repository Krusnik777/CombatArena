using System;
using CombatArena.Game.Configs;
using UnityEngine;
using R3;
using CombatArena.Game.Gameplay.HealthSystem;

namespace CombatArena.Game.Gameplay.Entities
{
    public class AOEDamageDealer : IDamageDealer
    {
        private LayerMask _targetMask;
        private Transform _transform;
        private AttackAbilityConfig _attackConfig;
        private AnimatorEventsCollector _eventsCollector;

        private IDisposable _attackExecutedListenerDisposable;

        public AOEDamageDealer(LayerMask targetMask, Transform transform, AttackAbilityConfig config, AnimatorEventsCollector eventsCollector)
        {
            _targetMask = targetMask;
            _transform = transform;
            _attackConfig = config;
            _eventsCollector = eventsCollector;

            _attackExecutedListenerDisposable = _eventsCollector.OnAttackExecute.Subscribe(_ => DamageAllInRange());
        }

        public void Dispose()
        {
            _attackExecutedListenerDisposable?.Dispose();
        }

        private void DamageAllInRange()
        {
            _attackExecutedListenerDisposable?.Dispose();

            var damage = DamageFactory.Create(_attackConfig);

            Collider[] colliders = Physics.OverlapSphere(_transform.position, _attackConfig.Range, _targetMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                var damageable = colliders[i].transform.GetComponent<IDamageable>();

                damageable?.Hit(damage);
            }
        }
    }
}
