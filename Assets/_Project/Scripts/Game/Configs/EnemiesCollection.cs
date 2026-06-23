using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "EnemiesCollection", menuName = "Scriptable Objects/Enemies Collection")]
    public class EnemiesCollection : ScriptableObject
    {
        [field: SerializeField] public EnemyConfig[] AllEnemies { get; private set; }

        public EnemyConfig GetConfig(string id)
        {
            for (int i = 0; i < AllEnemies.Length; i++)
            {
                if (AllEnemies[i].ID == id) return AllEnemies[i];
            }

            return null;
        }
    }
}
