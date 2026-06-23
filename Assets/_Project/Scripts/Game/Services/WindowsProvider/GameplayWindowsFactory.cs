using System;
using CombatArena.Game.Gameplay.UI;
using UnityEngine;
using Screen = UI.Windows.Screen;
using WindowView = UI.Windows.WindowView;

namespace CombatArena.Game.Services
{
    public class GameplayWindowsFactory : IWindowsFactory
    {
        private string _battleScreenViewName = "BattleScreenView";

        private Transform _screensHolder;
        private Transform _popupsHolder;

        public GameplayWindowsFactory(Transform screensHolder, Transform popupsHolder)
        {
            _screensHolder = screensHolder;
            _popupsHolder = popupsHolder;
        }

        public virtual T CreateScreen<T>() where T : Screen
        {
            Type t = typeof(T);

            if (t == typeof(BattleScreen))
            {
                var prefabPath = GetPrefabPath(_battleScreenViewName);
                var view = InstantiateWindowViewForScreen<BattleScreenView>(prefabPath);

                return new BattleScreen(view) as T;
            }

            throw new ArgumentNullException($"Unsupported class - type of: {t}");
        }

        /*public virtual T CreatePopup<T>() where T : Popup
        {
            Type t = typeof(T);

            if (t == typeof(SettingsWindow))
            {
                var prefabPath = GetCommonPopupPrefabPath(_settingsWindowViewName);
                var view = InstantiateWindowViewForPopup<SettingsWindowView>(prefabPath);

                return new SettingsWindow(view) as T;
            }

            if (t == typeof(ConfirmWindow))
            {
                var prefabPath = GetCommonPopupPrefabPath(_confirmWindowViewName);
                var view = InstantiateWindowViewForPopup<ConfirmWindowView>(prefabPath);

                return new ConfirmWindow(view) as T;
            }

            throw new ArgumentNullException($"Unsupported class - type of: {t}");
        }*/

        private T InstantiateWindowViewForScreen<T>(string prefabPath) where T : WindowView
        {
            var prefab = Resources.Load<T>(prefabPath);
            var windowView = GameObject.Instantiate(prefab, _screensHolder);

            return windowView;
        }

        private string GetPrefabPath(string viewName)
        {
            return $"Prefabs/UI/Gameplay/Screens/{viewName}";
        }
    }
}