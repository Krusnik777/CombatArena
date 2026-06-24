using DI;
using StateMachine;

namespace CombatArena.Game.StateMachines
{
    public class GameplayStateMachine : AbstractStateMachine
    {
        public GameplayStateMachine(DIContainer sceneContainer)
        {
            _states = new()
            {
                [typeof(BeforeBattleState)] = new BeforeBattleState(this, sceneContainer), 
                [typeof(ActiveBattleState)] = new ActiveBattleState(this, sceneContainer), 
                [typeof(BattleResultState)] = new BattleResultState(this, sceneContainer)
            };
        }
    }
}
