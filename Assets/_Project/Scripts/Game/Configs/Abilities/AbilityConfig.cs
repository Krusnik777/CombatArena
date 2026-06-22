using UnityEngine;

namespace CombatArena.Game.Configs
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        #if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (Cooldown < 0f) Cooldown = 0f;
        }

        #endif
    }
}
