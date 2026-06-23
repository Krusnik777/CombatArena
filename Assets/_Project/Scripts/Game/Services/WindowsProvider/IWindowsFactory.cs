using Screen = UI.Windows.Screen;
//using Popup = UI.Windows.Popup;

namespace CombatArena.Game.Services
{
    public interface IWindowsFactory
    {
        public T CreateScreen<T>() where T : Screen;
    }
}
