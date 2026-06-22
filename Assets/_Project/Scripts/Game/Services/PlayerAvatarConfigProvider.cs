using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class PlayerAvatarConfigProvider
    {
        public PlayerAvatarConfig Config { get; }

        public PlayerAvatarConfigProvider()
        {
            Config = Resources.Load<PlayerAvatarConfig>("Settings/PlayerAvatarConfig");
        }

    }
}