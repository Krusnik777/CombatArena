using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Enemy
{
    public class EnemyFactory
    {
        private EnemiesCollection _enemiesCollection;

        public EnemyFactory(EnemiesCollection enemiesCollection)
        {
            _enemiesCollection = enemiesCollection;
        }

        public Enemy CreateRandomEnemy(Vector3 position)
        {
            int index = Random.Range(0, _enemiesCollection.AllEnemies.Length);
            var enemyConfig = _enemiesCollection.AllEnemies[index];
            
            return CreateEnemy(enemyConfig, position);
        }

        public Enemy CreateEnemy(string configId, Vector3 position)
        {
            var enemyConfig = _enemiesCollection.GetConfig(configId);

            if (enemyConfig == null) throw new System.NullReferenceException($"[Enemy Factory] Couldn't find config by id: {configId}");

            return CreateEnemy(enemyConfig, position);
        }

        public Enemy CreateEnemy(EnemyConfig enemyConfig, Vector3 position)
        {
            var enemyPrefabPath = _enemiesCollection.PrefabsPath + "/" + enemyConfig.PrefabName;
            var enemyViewPrefab = Resources.Load<EnemyView>(enemyPrefabPath);

            if (enemyViewPrefab == null) throw new System.NullReferenceException(
                    $"[Enemy Factory] Couldn't find prefab by name {enemyConfig.PrefabName} in path {_enemiesCollection.PrefabsPath} for config id {enemyConfig.ID}");

            var enemyView = GameObject.Instantiate(enemyViewPrefab, position, Quaternion.identity);
            var enemy = new Enemy(enemyConfig, enemyView);

            return enemy;
        }
    }
}
