using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "PlayerAvatarConfig", menuName = "Scriptable Objects/Player/Player Avatar Config")]
    public class PlayerAvatarConfig : ScriptableObject
    {
        [field: Header("Movement")]
        [field: SerializeField] public float MovementSpeed { get; private set; } = 7f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 250f;
        [field: SerializeField] public bool IsMovementIsometric  { get; private set; } = true;
        [field: SerializeField] public bool IsInputReverseMovement { get; private set; } = false;
        [field: Header("Detection")]
        [field: SerializeField] public float EnemyDetectionRange { get; private set; } = 5f;
    }
}
