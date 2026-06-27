using CombatArena.Game.Root;
using R3;

namespace CombatArena.Game.Gameplay.UI
{
    public class VictoryScreen : ResultScreen
    {
        private VictoryScreenView _concreteView => _view as VictoryScreenView;

        public VictoryScreen(VictoryScreenView view) : base(view) { }

        protected override void SubscribeToButtons()
        {
            _buttonListenerDisposables?.Dispose();

            _buttonListenerDisposables = new()
            {
                _concreteView.OnAcceptPress.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.NEXT)),
                _concreteView.OnDeclinePress.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.EXIT))
            };

            if (_bindedGameInputService != null)
            {
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnSubmitPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.NEXT)));
                _buttonListenerDisposables.Add(_bindedGameInputService.UIInputController.OnCancelPressed.Subscribe(_ => _onChoseMade.OnNext(GameplayTags.EXIT)));
            }
        }
    }
}
