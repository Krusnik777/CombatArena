using System;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemySpawner : IDisposable
    {
        public Subject<Enemy> OnEnemySpawned { get; }

        private EnemySpawnerView _view;
        private EnemyFactory _enemyFactory;
        private Func<bool> _isSpawnAllowed;

        private LayerMask _checkMask;

        private int _currentTime;

        private IDisposable _secondsCounterDisposable;

        public EnemySpawner(EnemySpawnerView view, EnemyFactory enemyFactory, Func<bool> isSpawnAllowed = null)
        {
            _view = view;
            _enemyFactory = enemyFactory;
            _isSpawnAllowed = isSpawnAllowed ?? (() => true);

            OnEnemySpawned = new();

            _checkMask = Root.LayerMasks.Enemy.value | Root.LayerMasks.Player.value;

            _currentTime = _view.SpawnAtStart ? _view.SecondsUntilNextSpawn : 0;

            _secondsCounterDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => CountAndTryToSpawn());
        }

        public void Dispose()
        {
            _secondsCounterDisposable?.Dispose();
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

            var enemy = _enemyFactory.CreateRandomEnemy(_view.SpawnPoint.position);
            OnEnemySpawned.OnNext(enemy);
        }
    }
}
