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

        private const int _defaultTargetEnemiesDeaths = 10;

        private GameplayLevelView _view;
        
        private ReactiveProperty<int> _targetEnemiesDeaths;

        private List<Enemy> _spawnedEnemies;
        
        private CompositeDisposable _spawnerDisposables;

        public GameplayLevelController(GameplayLevelView view, int runs)
        {
            _view = view;
            _targetEnemiesDeaths = new (_defaultTargetEnemiesDeaths * (runs + 1));
        }

        public void Dispose()
        {
            _spawnerDisposables?.Dispose();
        }

        public Observable<Unit> OnPlayerEnteredToArena()
        {
            return _view.ArenaEnterTrigger.OnEnter;
        }

        public void SetEnterGateEnabled(bool state)
        {
            _view.EnterGate.SetActive(state);
        }

        public void StartSpawners(EnemyFactory enemyFactory, Transform pursueTarget)
        {
            _spawnerDisposables?.Dispose();

            _spawnerDisposables = new();
            _spawnedEnemies = new();

            for (int i = 0; i < _view.EnemySpawnerViews.Length; i++)
            {
                var spawner = new EnemySpawner(_view.EnemySpawnerViews[i], enemyFactory);
                _spawnerDisposables.Add(spawner);
                _spawnerDisposables.Add(spawner.OnEnemySpawned.Subscribe(enemy =>
                {
                    enemy.AssignPursueTarget(pursueTarget);
                    
                    _spawnerDisposables.Add(enemy.OnDeath.Subscribe(OnEnemyDead));
                    _spawnedEnemies.Add(enemy);
                }));
            }
        }

        public void StopSpawnersAndEnemies()
        {
            _spawnerDisposables?.Dispose();

            foreach (var enemy in _spawnedEnemies)
            {
                enemy.Stop();
            }
        }

        private void OnEnemyDead(Enemy enemy)
        {
            if (!_spawnedEnemies.Contains(enemy)) return;

            _spawnedEnemies.Remove(enemy);

            _targetEnemiesDeaths.Value--;
        }
    }
}
