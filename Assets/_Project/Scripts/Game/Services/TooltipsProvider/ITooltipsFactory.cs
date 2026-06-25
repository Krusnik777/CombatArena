using UI.Tooltips;

namespace CombatArena.Game.Services
{
    public interface ITooltipsFactory
    {
        public TooltipView CreateTooltipView(TooltipType type);
    }
}
