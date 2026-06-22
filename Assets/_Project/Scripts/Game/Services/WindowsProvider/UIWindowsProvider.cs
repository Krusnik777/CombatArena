using System;
using System.Collections.Generic;
using System.Linq;
using UI.Windows;

namespace CombatArena.Game.Services
{
    public class UIWindowsProvider : IDisposable
    {
        private IWindowsFactory _windowsFactory;

        private Dictionary<Type, Screen> _createdScreensMap;
        private Screen _activeScreen;

        private Dictionary<Popup, Type> _openedPopupsMap;

        public UIWindowsProvider(IWindowsFactory windowsFactory)
        {
            _windowsFactory = windowsFactory;

            _createdScreensMap = new();
            _openedPopupsMap = new();
        }

        public T GetScreen<T>() where T : Screen
        {
            if (_activeScreen is T) return _activeScreen as T;

            _activeScreen?.Hide();

            if (!_createdScreensMap.ContainsKey(typeof(T)))
            {
                var newScreen = _windowsFactory.CreateScreen<T>();
                _createdScreensMap.Add(typeof(T), newScreen);
            }

            var screen = _createdScreensMap[typeof(T)] as T;
            _activeScreen = screen;

            return screen;
        }

        public T ShowScreen<T>() where T : Screen
        {
            var screen = GetScreen<T>();

            screen.Show();

            return screen;
        }

        public T ShowPopup<T>(IPopupInitData initData = null) where T : Popup
        {
            var popup = GetPopup<T>();

            popup.Initialize(initData);
            popup.Show();

            return popup;
        }

        public void Dispose()
        {
            foreach (var screen in _createdScreensMap)
            {
                screen.Value?.Dispose();
            }

            foreach (var popup in _openedPopupsMap)
            {
                popup.Key?.Dispose();
            }
        }

        private T GetPopup<T>() where T : Popup
        {
            if (_openedPopupsMap.ContainsValue(typeof(T)))
            {
                var popup = _openedPopupsMap.FirstOrDefault(x => x.Value == typeof(T)).Key;

                if (!popup.IsMultipleInstancesAllowed) return popup as T;
            }

            var newPopup = _windowsFactory.CreatePopup<T>();
            _openedPopupsMap.Add(newPopup, typeof(T));

            newPopup.SubscribeToClose(OnPopupClosed);

            return newPopup;
        }

        private void OnPopupClosed(Popup popup)
        {
            if (!_openedPopupsMap.ContainsKey(popup)) return;

            _openedPopupsMap.Remove(popup);

            popup.Dispose();
        }
    }
}
