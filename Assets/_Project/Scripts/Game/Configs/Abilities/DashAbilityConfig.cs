using UnityEngine;

namespace CombatArena.Game.Configs
{
    [CreateAssetMenu(fileName = "DashAbilityConfig", menuName = "Scriptable Objects/Abilities/Dash Ability Config")]
    public class DashAbilityConfig : AbilityConfig
    {
        [field: SerializeField] public float DashDistance { get; private set; } = 5f;
        [field: SerializeField] public int DashSteps { get; private set; } = 10;
        [field: SerializeField] public float DashSpeedMultiplier { get; private set; } = 0.5f;

        public override AbilityType Type => AbilityType.Dash;
        
        #if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (DashDistance < 1) DashDistance = 1;
            if (DashSteps < 1) DashSteps = 1;
        }

        #endif
    
    }
}
