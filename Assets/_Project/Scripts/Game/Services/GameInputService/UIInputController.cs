using System;
using R3;
using UnityEngine.InputSystem;

namespace CombatArena.Game.Services
{
    public class UIInputController : IDisposable
    {
        public Subject<Unit> OnUISubmitPressed { get; private set; } = new();

        private GameInput _gameInput;

        public UIInputController(GameInput gameInput)
        {
            _gameInput = gameInput;
            _gameInput.UI.Enable();

            _gameInput.UI.Submit.performed += OnSubmit;
        }

        public void Dispose()
        {
            _gameInput.UI.Submit.performed -= OnSubmit;
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            OnUISubmitPressed?.OnNext(Unit.Default);
        }
    }
}
