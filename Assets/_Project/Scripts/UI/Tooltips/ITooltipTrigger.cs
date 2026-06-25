using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public interface ITooltipTrigger : IPointerEnterHandler, IPointerExitHandler
    {
        public void BindTooltipHandler(ITooltipHandler tooltipHandler);
        public TooltipData GetTooltipData();
    }
}
