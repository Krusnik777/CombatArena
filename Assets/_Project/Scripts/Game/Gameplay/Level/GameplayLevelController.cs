using System;
using System.Collections.Generic;
using CombatArena.Game.Gameplay.Entities.Enemies;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Levels
{
    public class GameplayLevelController : IDisposable
    {
        public Observable<int> OnEnemyDied => _targetEnemiesDeaths;
        public Observable<Enemy> EnemyDetectedByPlayer => _detectedEnemyByPlayer;

        private const int _minTargetEnemiesDeaths = 10;
        private const int _maxTargetEnemiesDeaths = 50;
        
        private const int _maxSpawnedEnemiesOnScene = 10;
        private const int _enemiesExcess = 5;

        private GameplayLevelView _view;

        private ReactiveProperty<int> _targetEnemiesDeaths;
        private ReactiveProperty<Enemy> _detectedEnemyByPlayer;

        private List<EnemySpawner> _spawners;
        private Dictionary<Enemy, (IDisposable, IDisposable)> _spawnedEnemiesMap;
        private EnemyPool _enemyPool;
        
        private CompositeDisposable _spawnerDisposables;
        private IDisposable _enemyDetectionListenerDisposable;

        public GameplayLevelController(GameplayLevelView view, int runs)
        {
            _view = view;
            _targetEnemiesDeaths = new(runs > 4 ? _maxTargetEnemiesDeaths : _minTargetEnemiesDeaths * (runs + 1));
            _detectedEnemyByPlayer = new();
            
            _spawners = new();
        }

        public void Dispose()
        {
            _spawnerDisposables?.Dispose();
            _enemyDetectionListenerDisposable?.Dispose();

            foreach (var disposablePair in _spawnedEnemiesMap.Values)
            {
                disposablePair.Item1?.Dispose();
                disposablePair.Item2?.Dispose();
            }
        }

        public Observable<Unit> OnPlayerEnteredToArena()
        {
            return _view.ArenaEnterTrigger.OnEnter;
        }

        public void AssignEnemyDetector(IEnemyDetector enemyDetector)
        {
            _enemyDetectionListenerDisposable?.Dispose();

            _enemyDetectionListenerDisposable = enemyDetector.DetectedEnemyView.Subscribe(enemyView =>
            {
                if (enemyView == null)
                {
                    _detectedEnemyByPlayer.Value = null;
                    return;
                }

                foreach (var enemy in _spawnedEnemiesMap.Keys)
                {
                    if (enemy.IsSameView(enemyView))
                    {
                        _detectedEnemyByPlayer.Value = enemy;

                        return;
                    }
                }

                _detectedEnemyByPlayer.Value = null;
            });
        }

        public void SetEnterGateEnabled(bool state)
        {
            _view.EnterGate.SetActive(state);
        }

        public void PrepareSpawners(EnemyPool enemyPool, SimpleGameObjectsPool particlesPool)
        {
            _spawners.Clear();
            _spawnerDisposables?.Dispose();
            _enemyPool?.Dispose();

            _spawnerDisposables = new();
            _spawnedEnemiesMap = new();
            _enemyPool = enemyPool;

            for (int i = 0; i < _view.EnemySpawnerViews.Length; i++)
            {
                var spawner = new EnemySpawner(_view.EnemySpawnerViews[i], enemyPool, IsSpawnAllowed);
                _spawners.Add(spawner);
                _spawnerDisposables.Add(spawner);
                _spawnerDisposables.Add(spawner.OnEnemySpawned.Subscribe(enemy =>
                {
                    enemy.ActivateParticles(particlesPool);
    
                    _spawnedEnemiesMap.Add(enemy, (enemy.OnDeath.Subscribe(OnEnemyDead), enemy.OnDeleted.Subscribe(OnEnemyDeleted)));

                    if (_spawnedEnemiesMap.Count > _targetEnemiesDeaths.Value + _enemiesExcess) _spawnerDisposables?.Dispose();
                }));
            }
        }

        public void StartSpawners(Transform pursueTarget)
        {
            for (int i = 0; i < _spawners.Count; i++)
            {
                _spawners[i].Start(pursueTarget);
            }
        }

        public void StopSpawnersAndEnemies(bool killRemainingEnemies)
        {
            _spawnerDisposables?.Dispose();
            _enemyDetectionListenerDisposable?.Dispose();

            foreach (var enemy in _spawnedEnemiesMap)
            {
                enemy.Key.CleanupHitNumbers();
                
                if (killRemainingEnemies) enemy.Key.Kill();
                else enemy.Key.Stop();

                enemy.Value.Item1?.Dispose();
                enemy.Value.Item2?.Dispose();
            }
        }

        private bool IsSpawnAllowed() => _spawnedEnemiesMap.Count < _maxSpawnedEnemiesOnScene;

        private void OnEnemyDead(Enemy enemy)
        {
            if (!_spawnedEnemiesMap.ContainsKey(enemy)) return;

            _spawnedEnemiesMap[enemy].Item1.Dispose();

            _targetEnemiesDeaths.Value--;
        }

        private void OnEnemyDeleted(Enemy enemy)
        {
            if (!_spawnedEnemiesMap.ContainsKey(enemy)) return;

            _enemyPool.Return(enemy);

            _spawnedEnemiesMap[enemy].Item2.Dispose();
            _spawnedEnemiesMap.Remove(enemy);
        }
    }
}
