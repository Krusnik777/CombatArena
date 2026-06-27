using System;
using System.Collections.Generic;
using CombatArena.Game.Configs;
using CombatArena.Game.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CombatArena.Game.Gameplay.Entities.Enemies
{
    public class EnemyPool : IDisposable
    {
        private EnemiesCollection _enemiesCollection;
        private SoundService _soundService;

        private SimpleGameObjectsPool _viewsPool;
        private Dictionary<Enemy, EnemyView> _enemiesMap;

        public EnemyPool(EnemiesCollection enemiesCollection, SoundService soundService, Transform poolParent)
        {
            _enemiesCollection = enemiesCollection;
            _soundService = soundService;

            _viewsPool = new SimpleGameObjectsPool(poolParent);
            _enemiesMap = new();

            for (int i = 0; i < _enemiesCollection.AllEnemies.Length; i++)
            {
                var enemyConfig = _enemiesCollection.AllEnemies[i];
                var enemyPrefabPath = _enemiesCollection.PrefabsPath + "/" + enemyConfig.PrefabName;
                var enemyViewPrefab = Resources.Load<EnemyView>(enemyPrefabPath);

                if (enemyViewPrefab == null) throw new System.NullReferenceException(
                        $"[Enemy Factory] Couldn't find prefab by name {enemyConfig.PrefabName} in path {_enemiesCollection.PrefabsPath} for config id {enemyConfig.ID}");

                _viewsPool.Add(enemyViewPrefab.gameObject);
            }
        }

        public void Dispose()
        {
            _viewsPool?.Dispose();
        }

        public Enemy GetRandomEnemy(Vector3 position)
        {
            int index = Random.Range(0, _enemiesCollection.AllEnemies.Length);
            var enemyConfig = _enemiesCollection.AllEnemies[index];

            var enemyView = _viewsPool.Get(enemyConfig.PrefabName).GetComponent<EnemyView>();
            enemyView.Animator.ResetControllerState();

            enemyView.transform.position = position;
            enemyView.Damageable.gameObject.SetActive(true);
            enemyView.Agent.enabled = true;

            var enemy = new Enemy(enemyConfig, enemyView, _soundService);
            _enemiesMap.Add(enemy, enemyView);

            return enemy;
        }

        public void Return(Enemy enemy)
        {
            if (!_enemiesMap.ContainsKey(enemy)) return;

            var view = _enemiesMap[enemy];
            view.Agent.enabled = false;

            _viewsPool.Return(_enemiesMap[enemy].gameObject);
            _enemiesMap.Remove(enemy);

            enemy?.Dispose();
        }
    }
}
