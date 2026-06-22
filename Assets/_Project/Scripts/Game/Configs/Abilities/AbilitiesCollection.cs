using UnityEngine;

namespace CombatArena.Game.Configs
{
    [System.Serializable]
    public class AbilityConfigsBundle
    {
        public AbilityConfig AbilityA;
        public AbilityConfig AbilityX;
        public AbilityConfig AbilityY;
    }

    [CreateAssetMenu(fileName = "AbilitiesCollection", menuName = "Scriptable Objects/Abilities Collection")]
    public class AbilitiesCollection : ScriptableObject
    {
        [field: SerializeField] public AbilityConfig[] AllAbilities { get; private set; }
        [field: SerializeField] public AbilityConfigsBundle DefaultAbilitiesBundle { get; private set; } 
    }
}
