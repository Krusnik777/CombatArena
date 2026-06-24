using UnityEngine;

namespace CombatArena.Game.Configs
{
    public enum AttackType
    {
        BasicAttack,
        AreaAttack
    }

    [CreateAssetMenu(fileName = "AttackAbilityConfig", menuName = "Scriptable Objects/Abilities/Attack Ability Config")]
    public class AttackAbilityConfig : AbilityConfig
    {
        [field: SerializeField] public AttackType AttackType { get; private set; }
        [field: SerializeField] public int Damage { get ; private set; } = 1;
        [field: SerializeField] public float Range { get ; private set; } = 3;
        [field: SerializeField][field: Range(0f, 1f)] public float CriticalChance { get; private set; } = 0.15f;
        [field: SerializeField][field: Range(0f, 1f)] public float ArmorBreakChance { get; private set; } = 0.15f;

        public override AbilityType Type => AbilityType.Attack;
        
        #if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Damage < 1) Damage = 1;
            if (Range < 0) Range = 1;
        }

        #endif
    }
}
