namespace CombatArena.Game.Root.Sounds
{
    public static class EnemiesSounds
    {
        public static readonly SoundData CommonDamage = new(_commonPath + "enemies_damage", 0.5f);

        public static readonly SoundData GoblinAttack = new(_commonPath + "goblin_attack", 0.5f);
        public static readonly SoundData GoblinDeath = new(_commonPath + "goblin_death", 0.5f);

        public static readonly SoundData HobgoblinAttack = new(_commonPath + "hobgoblin_attack", 0.5f);
        public static readonly SoundData HobgoblinDeath = new(_commonPath + "hobgoblin_death", 0.5f);

        public static readonly SoundData SkeletonAttack = new(_commonPath + "skeleton_attack", 0.5f);
        public static readonly SoundData SkeletonDamage = new(_commonPath + "skeleton_damage", 0.5f);
        public static readonly SoundData SkeletonDeath = new(_commonPath + "skeleton_death", 0.5f);

        public static readonly SoundData TrollAttack = new(_commonPath + "troll_attack", 0.5f);
        public static readonly SoundData TrollDeath = new(_commonPath + "troll_death", 0.5f);

        private const string _commonPath = "Enemies/";
    }
}
