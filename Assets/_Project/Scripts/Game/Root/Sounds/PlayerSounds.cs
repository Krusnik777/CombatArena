namespace CombatArena.Game.Root.Sounds
{
    public static class PlayerSounds
    {
        public static readonly SoundData Defeat = new("defeat", 1f);

        public static readonly SoundData[] Steps = new SoundData[]
        {
            new(_commonPath + "Steps/player_step_1", 0.2f),
            new(_commonPath + "Steps/player_step_2", 0.2f),
            new(_commonPath + "Steps/player_step_3", 0.2f)
        };

        public static readonly SoundData Attack = new(_commonPath + "player_basic_attack", 0.35f);
        public static readonly SoundData AreaAttack = new(_commonPath + "player_super_attack", 0.35f);
        public static readonly SoundData Damage = new(_commonPath + "player_damage", 0.5f);
        public static readonly SoundData Dash = new(_commonPath + "player_dash", 0.5f);
        public static readonly SoundData Death = new(_commonPath + "player_death", 0.5f);
        public static readonly SoundData Heal = new(_commonPath + "player_heal", 0.5f);

        private const string _commonPath = "Player/";
    }
}
