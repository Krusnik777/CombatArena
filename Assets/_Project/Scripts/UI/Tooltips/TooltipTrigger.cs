using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public abstract class TooltipTrigger : MonoBehaviour, ITooltipTrigger
    {
        protected abstract TooltipType _tooltipeType { get; }

        private ITooltipHandler _tooltipHandler;

        private bool _entered;

        public abstract TooltipData GetTooltipData();

        public void BindTooltipHandler(ITooltipHandler tooltipHandler)
        {
            _tooltipHandler = tooltipHandler;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var data = GetTooltipData();

            if (data == null) return;

            _tooltipHandler?.ShowTooltip(_tooltipeType, data);

            _entered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltipHandler?.HideTooltip();

            _entered = false;
        }

        protected void ChangeTooltipIfActive()
        {
            if (_entered)
            {
                var data = GetTooltipData();

                if (data == null) return;

                _tooltipHandler?.ShowTooltip(_tooltipeType, data);
            }
        }

        private void OnDisable()
        {
            if (_entered)
            {
                _tooltipHandler?.HideTooltip();

                _entered = false;
            }
        }
    }
}
