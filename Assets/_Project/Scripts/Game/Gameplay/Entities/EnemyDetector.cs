using System;
using CombatArena.Game.Gameplay.Entities.Enemies;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities
{
    public class EnemyDetector : IEnemyDetector
    {
        public Observable<EnemyView> DetectedEnemyView => _detectedEnemyView;
        
        private LayerMask _enemyMask;
        private Transform _transform;
        private float _detectionRange;

        private ReactiveProperty<EnemyView> _detectedEnemyView;

        private IDisposable _updateDisposable;

        public EnemyDetector(LayerMask enemyMask, Transform transform, float detectionRange)
        {
            _enemyMask = enemyMask;
            _transform = transform;
            _detectionRange = detectionRange;

            _detectedEnemyView = new();

            _updateDisposable = Observable.Interval(TimeSpan.FromSeconds(0.25f)).Subscribe(_ => TryDetectEnemy());
        }

        public void Dispose()
        {
            _updateDisposable?.Dispose();
        }

        private void TryDetectEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, _detectionRange, _enemyMask);

            EnemyView target = null;
            float targetDistance = float.MaxValue;

            for (int i = 0; i < colliders.Length; i++)
            {
                var potentialTarget = colliders[i].transform.root.GetComponent<EnemyView>();

                if (potentialTarget == null) continue;

                var dist = Vector3.Distance(potentialTarget.transform.position, _transform.position);

                if (dist < targetDistance)
                {
                    targetDistance = dist;
                    target = potentialTarget;
                }
            }

            if (_detectedEnemyView.Value == target) return;

            if (_detectedEnemyView.Value != null) _detectedEnemyView.Value.ChosenEffect.SetActive(false);
            if (target != null) target.ChosenEffect.SetActive(true);

            _detectedEnemyView.Value = target;
        }
    }
}
