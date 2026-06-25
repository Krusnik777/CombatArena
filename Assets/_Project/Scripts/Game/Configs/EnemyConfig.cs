using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string PrefabName { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; } = 50;
        [field: SerializeField][field: Range(0f, 1f)] public float ArmorDefenceChance { get; private set; } = 0.05f;
        [field: SerializeField] public int Armor { get; private set; } = 5;
        [field: SerializeField] public AttackAbilityConfig AttackAbility { get; private set; } // temp?
        [field: Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 5f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 250f;
        //[field: SerializeField] public float StoppingDistance { get; private set; } = 2f;
    }
}
