using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "DamageableMasksConfig", menuName = "Scriptable Objects/Damageable Masks Config")]
    public class DamageableMasksConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask Player { get; private set; }
        [field: SerializeField] public LayerMask Enemy { get; private set; }
    }
}
