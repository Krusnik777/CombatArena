using System;

namespace CombatArena
{
    public class EventInvoker : IEventInvoker
    {
        private Action _bindedAction;

        public EventInvoker(Action action)
        {
            _bindedAction = action;
        }

        public void InvokeBindedAction()
        {
            _bindedAction?.Invoke();
        }
    }
}