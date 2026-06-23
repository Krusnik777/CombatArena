using CombatArena.Game.Configs;
using UnityEngine;

namespace CombatArena.Game.Services
{
    public class PlayerConfigsProvider
    {
        public PlayerAvatarConfig AvatarConfig { get; }
        public PlayerHealthConfig HealthConfig { get; }

        public PlayerConfigsProvider()
        {
            AvatarConfig = Resources.Load<PlayerAvatarConfig>("Settings/PlayerAvatarConfig");
            HealthConfig = Resources.Load<PlayerHealthConfig>("Settings/PlayerHealthConfig");
        }
    }
}