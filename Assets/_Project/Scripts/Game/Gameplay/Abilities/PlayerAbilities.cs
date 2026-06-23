namespace CombatArena.Game.Gameplay
{
    public class PlayerAbilities : System.IDisposable
    {
        public Ability AbilityA { get; }
        public Ability AbilityX { get; }
        public Ability AbilityY { get; }

        public PlayerAbilities(Ability abilityA, Ability abilityX, Ability abilityY)
        {
            AbilityA = abilityA;
            AbilityX = abilityX;
            AbilityY = abilityY;
        }

        public void Dispose()
        {
            AbilityA?.Dispose();
            AbilityX?.Dispose();
            AbilityY?.Dispose();
        }
    }
}