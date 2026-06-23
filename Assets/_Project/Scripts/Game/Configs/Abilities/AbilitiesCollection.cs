using UnityEngine;

namespace CombatArena.Game.Configs
{
    [System.Serializable]
    public class PlayerAbilityConfigs
    {
        public AbilityConfig AbilityA;
        public AbilityConfig AbilityX;
        public AbilityConfig AbilityY;
    }

    [CreateAssetMenu(fileName = "AbilitiesCollection", menuName = "Scriptable Objects/Abilities Collection")]
    public class AbilitiesCollection : ScriptableObject
    {
        [field: SerializeField] public AbilityConfig[] AllAbilities { get; private set; }
        [field: SerializeField] public PlayerAbilityConfigs DefaultPlayerAbilities { get; private set; } 

        public AbilityConfig GetConfig(string id)
        {
            for (int i = 0; i < AllAbilities.Length; i++)
            {
                if (AllAbilities[i].ID == id) return AllAbilities[i];
            }

            return null;
        }
    }
}
