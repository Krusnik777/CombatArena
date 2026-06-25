using CombatArena.Game.Gameplay.Entities.Enemies;
using R3;

namespace CombatArena.Game.Gameplay.Entities
{
    public interface IEnemyDetector : System.IDisposable
    {
        public Observable<EnemyView> DetectedEnemyView { get; }
    }
}
