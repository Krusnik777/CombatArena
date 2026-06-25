using UI.Tooltips;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class TooltipsFactory : ITooltipsFactory
    {
        private const string _abilityToolTipViewName = "AbilityTooltipView";
        private const string _healthToolTipViewName = "HealthTooltipView";

        private Transform _canvasTransform;

        public TooltipsFactory(Transform tooltipsCanvasTransform)
        {
            _canvasTransform = tooltipsCanvasTransform;
        }

        public TooltipView CreateTooltipView(TooltipType type)
        {
            switch(type)
            {
                case TooltipType.Ability:
                return InstantiateTooltipView(GetPrefabPath(_abilityToolTipViewName));

                case TooltipType.Health:
                return InstantiateTooltipView(GetPrefabPath(_healthToolTipViewName));
            }

            throw new System.ArgumentOutOfRangeException($"Current type for tooltip not supported: {type}");
        }

        private TooltipView InstantiateTooltipView(string prefabPath)
        {
            var prefab = Resources.Load<TooltipView>(prefabPath);
            var view = GameObject.Instantiate(prefab, _canvasTransform);

            return view;
        }

        private string GetPrefabPath(string viewName)
        {
            return $"Prefabs/UI/Tooltips/{viewName}";
        }
    }
}
