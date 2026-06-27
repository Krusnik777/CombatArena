using System;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemySpawner : IDisposable
    {
        public Subject<Enemy> OnEnemySpawned { get; }

        private EnemySpawnerView _view;
        private EnemyPool _enemyPool;
        private Func<bool> _isSpawnAllowed;

        private LayerMask _checkMask;

        private int _currentTime;

        private Enemy _nextPreparedEnemy;
        private Transform _pursueTarget;

        private IDisposable _secondsCounterDisposable;

        public EnemySpawner(EnemySpawnerView view, EnemyPool enemyPool, Func<bool> isSpawnAllowed = null)
        {
            _view = view;
            _enemyPool = enemyPool;
            _isSpawnAllowed = isSpawnAllowed ?? (() => true);

            OnEnemySpawned = new();

            _checkMask = Root.LayerMasks.Enemy.value | Root.LayerMasks.Player.value;

            _currentTime = _view.SpawnAtStart ? _view.SecondsUntilNextSpawn : 0;

            PrepareNext();
        }

        public void Dispose()
        {
            _secondsCounterDisposable?.Dispose();
            _nextPreparedEnemy?.Dispose();
        }

        public void Start(Transform pursueTarget)
        {
            _secondsCounterDisposable?.Dispose();

            _pursueTarget = pursueTarget;

            _secondsCounterDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => CountAndTryToSpawn());
        }

        private void CountAndTryToSpawn()
        {
            if (_currentTime >= _view.SecondsUntilNextSpawn)
            {
                TryToSpawn();
                return;
            }

            _currentTime++;
        }

        private void TryToSpawn()
        {
            if (!_isSpawnAllowed())
            {
                _currentTime = _view.SecondsUntilNextSpawn/2;

                return;
            }

            Collider[] colliders = Physics.OverlapSphere(_view.SpawnPoint.position, _view.AvailableCheckRange, _checkMask);

            if (colliders != null && colliders.Length > 0) return;

            _currentTime = 0;

            var enemy = _nextPreparedEnemy;
            enemy.ActivateAndAssignPurseTarget(_pursueTarget);
            
            OnEnemySpawned.OnNext(enemy);

            _view.Effect.Play();

            PrepareNext();
        }

        private void PrepareNext()
        {
            _nextPreparedEnemy = _enemyPool.GetRandomEnemy(_view.SpawnPoint.position);
            _nextPreparedEnemy.Disable();
        }
    }
}
