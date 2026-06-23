using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "PlayerHealthConfig", menuName = "Scriptable Objects/Player/Player Health Config")]
    public class PlayerHealthConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;
        [field: SerializeField][field: Range(0f, 1f)] public float ArmorDefenceChance { get; private set; } = 0.15f;
        [field: SerializeField] public int Armor { get; private set; } = 15;
    }
}
