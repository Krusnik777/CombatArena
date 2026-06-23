using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities
{
    public class AnimatorEventsCollector : MonoBehaviour
    {
        public Subject<Unit> OnAttackStart { get; private set;} = new();
        public Subject<Unit> OnAttackExecute { get; private set;} = new();
        public Subject<Unit> OnAttackFinish { get; private set;} = new();

        public void OnStep()
        {
            // Play Sound Footstep
            // Play Particle Footstep
        }

        public void OnAttackStarted()
        {
            OnAttackStart?.OnNext(Unit.Default);
        }

        public void OnAttackExecuted()
        {
            OnAttackExecute?.OnNext(Unit.Default);
        }

        public void OnAttackFinished()
        {
            OnAttackFinish?.OnNext(Unit.Default);
        }
    }
}
