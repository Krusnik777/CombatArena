using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CombatArena.Game.Services
{
    public class GameInputService : IDisposable
    {
        private GameInput _gameInput;
        public InputActionAsset ActionsAsset => _gameInput.asset;

        private UIInputController _uiInputController;
        public UIInputController UIInputController => _uiInputController;

        private IDisposable _anyButtonPressListenerDisposable;

        public GameInputService()
        {
            _gameInput = new();
            _gameInput.Enable();

            _uiInputController = new(_gameInput);
        }

        public void Dispose()
        {
            _anyButtonPressListenerDisposable?.Dispose();
            _uiInputController?.Dispose();
        }

        public void ClearReactionForAnyButtonPress() => _anyButtonPressListenerDisposable?.Dispose();

        public void SetReactionForAnyButtonPress(Action action)
        {
            _anyButtonPressListenerDisposable?.Dispose();

            _anyButtonPressListenerDisposable = InputSystem.onAnyButtonPress.Call(_ =>
            {
               _anyButtonPressListenerDisposable?.Dispose();

               action?.Invoke(); 
            });
        }
    }
}
