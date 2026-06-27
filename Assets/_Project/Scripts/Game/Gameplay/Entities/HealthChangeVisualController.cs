using System;
using System.Collections.Generic;
using System.Linq;
using CombatArena.Game.Gameplay.HealthSystem;
using CombatArena.Game.Gameplay.UI;
using R3;

namespace CombatArena.Game.Gameplay.Entities
{
    public class HealthChangeVisualController : IDisposable
    {
        private SimpleGameObjectsPool _pool;
        private Health _health;
        private IEffectsHolder _effectsHolder;

        private Dictionary<IDisposable, UIDamageInfo> _spawnedHitNumbersMap;

        private CompositeDisposable _healthChangesListenerDisposable;

        public HealthChangeVisualController(SimpleGameObjectsPool effectsPool, Health health, IEffectsHolder effectsHolder)
        {
            _pool = effectsPool;
            _health = health;
            _effectsHolder = effectsHolder;

            _spawnedHitNumbersMap = new();

            _pool.Add(effectsHolder.UIDamageInfoPrefab.gameObject);

            _healthChangesListenerDisposable = new()
            {
                health.OnDamage.Subscribe(OnDamage),
                health.OnHeal.Subscribe(OnHeal)
            };
        }

        public void Dispose()
        {
            _healthChangesListenerDisposable?.Dispose();

            foreach (var disposable in _spawnedHitNumbersMap.Keys) disposable?.Dispose();
        }

        public void ClearHitNumbers()
        {
            foreach (var spawned in _spawnedHitNumbersMap)
            {
                _pool.Return(spawned.Value.gameObject);
                spawned.Key?.Dispose();
            }

            _spawnedHitNumbersMap?.Clear();
        }

        private void OnDamage(Damage damage)
        {
            var uiDamage = _pool.Get(_effectsHolder.UIDamageInfoPrefab.gameObject).GetComponent<UIDamageInfo>();
            uiDamage.transform.position = _effectsHolder.HitTransform.position;

            var disposable = uiDamage.Show(damage).Subscribe(info =>
            {
                _pool.Return(info.gameObject);

                var findedDisposable = _spawnedHitNumbersMap.FirstOrDefault(v => v.Value == info).Key;
                findedDisposable?.Dispose();
                
                _spawnedHitNumbersMap.Remove(findedDisposable);
            });

            _spawnedHitNumbersMap.Add(disposable, uiDamage);

            _effectsHolder.HitEffect.Play();
        }

        private void OnHeal(int value)
        {
            
        }
    }
}
