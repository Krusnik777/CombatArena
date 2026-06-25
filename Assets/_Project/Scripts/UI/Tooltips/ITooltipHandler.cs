using UnityEngine;

namespace UI.Tooltips
{
    public interface ITooltipHandler
    {
        public void ShowTooltip(TooltipType type, TooltipData data);
        public void HideTooltip();
    }
}
