using System;
using CombatArena.Game.Configs;
using UnityEngine;
using R3;
using System.Collections.Generic;
using CombatArena.Game.Gameplay.HealthSystem;
using Random = UnityEngine.Random;

namespace CombatArena.Game.Gameplay.Entities
{
    public class DamageDealer : IDisposable
    {
        private Transform _transform;
        private AttackAbilityConfig _attackConfig;
        private AnimatorEventsCollector _eventsCollector;

        private IDisposable _attackExecutedListener;

        public DamageDealer(Transform transform, AttackAbilityConfig config, AnimatorEventsCollector eventsCollector)
        {
            _transform = transform;
            _attackConfig = config;
            _eventsCollector = eventsCollector;

            _attackExecutedListener = _eventsCollector.OnAttackExecute.Subscribe(_ => DamageAllInRange());
        }

        public void Dispose()
        {
            _attackExecutedListener?.Dispose();
        }

        private void DamageAllInRange()
        {
            _attackExecutedListener?.Dispose();

            bool isArmorBreak = Random.value >= 1 - _attackConfig.ArmorBreakChance;
            bool isCritical = Random.value >= 1 - _attackConfig.CriticalChance;

            var damage = new Damage()
            {
                BaseValue = _attackConfig.Damage,
                ResultValue = _attackConfig.Damage,
                Type = isArmorBreak ? DamageType.ArmorBreak : DamageType.Simple,
                Modifiers = new()
            };

            if (isCritical) damage.Modifiers.Add(new CriticalDamageModifier());

            Collider[] colliders = Physics.OverlapSphere(_transform.position, _attackConfig.Range);

            for (int i = 0; i < colliders.Length; i++)
            {
                var damageable = colliders[i].transform.GetComponent<IDamageable>();

                damageable?.Hit(damage);
            }
        }
    }
}
