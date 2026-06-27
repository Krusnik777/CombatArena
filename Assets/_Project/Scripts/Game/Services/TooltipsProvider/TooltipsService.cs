using System;
using System.Collections.Generic;
using R3;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CombatArena.Game.Services
{
    public class TooltipsService : ITooltipHandler, IDisposable
    {
        private class TooltipContext
        {
            public TooltipView view;
            public RectTransform rectTransform;
        }

        private readonly Canvas _canvas;
        private readonly RectTransform _canvasRectTransform;
        private readonly Vector2 _offset;
        private readonly TooltipsFactory _factory;

        private Dictionary<TooltipType, TooltipContext> _spawnedTooltipsMap;
        private TooltipContext _activeTooltip;

        private IDisposable _showDisposable;

        public TooltipsService(Canvas canvas, Vector2 offset, TooltipsFactory factory)
        {
            _canvas = canvas;
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
            _offset = offset;
            _factory = factory;

            _spawnedTooltipsMap = new();
        }

        public void Dispose()
        {
            _showDisposable?.Dispose();
        }

        public void ShowTooltip(TooltipType type, TooltipData data)
        {
            _showDisposable?.Dispose();

            _activeTooltip = GetTooltipContext(type);
            _activeTooltip.rectTransform.pivot = Vector2.zero;

            _activeTooltip.view.Setup(data);
            _activeTooltip.view.Show();

            _showDisposable = Observable.EveryUpdate().Subscribe(_ => UpdatePosition());
        }

        public void HideTooltip()
        {
            if (_activeTooltip == null) return;

            _showDisposable?.Dispose();

            _activeTooltip.view.Hide();
            _activeTooltip = null;
        }

        private TooltipContext GetTooltipContext(TooltipType type)
        {
            if (!_spawnedTooltipsMap.ContainsKey(type))
            {
                var spawnedView = _factory.CreateTooltipView(type);
                _spawnedTooltipsMap[type] = new TooltipContext
                {
                    view = spawnedView,
                    rectTransform = spawnedView.GetComponent<RectTransform>()
                };
            }

            return _spawnedTooltipsMap[type];
        }

        private void UpdatePosition()
        {
            Vector2 pointerPos = Pointer.current?.position.ReadValue() ?? Vector2.zero;
            SetPosition(pointerPos);
        }

        private void SetPosition(Vector2 screenPos)
        {
            if (_activeTooltip == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPos, _canvas.worldCamera, out Vector2 localPos);

            localPos += _offset;

            Vector2 canvasSize = _canvasRectTransform.rect.size;
            Vector2 tooltipSize = _activeTooltip.rectTransform.rect.size;

            float leftBound = -canvasSize.x / 2;
            float rightBound = canvasSize.x / 2;
            float bottomBound = -canvasSize.y / 2;
            float topBound = canvasSize.y / 2;

            if (localPos.x + tooltipSize.x > rightBound)
                localPos.x = rightBound - tooltipSize.x;
            if (localPos.x < leftBound)
                localPos.x = leftBound;

            if (localPos.y + tooltipSize.y > topBound)
                localPos.y = topBound - tooltipSize.y;
            if (localPos.y < bottomBound)
                localPos.y = bottomBound;

            _activeTooltip.rectTransform.anchoredPosition = localPos;
        }
    }
}
