using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities
{
    public class AnimatorEventsCollector : MonoBehaviour
    {
        public Subject<int> OnFootstep { get; private set;} = new();
        public Subject<int> OnAttackStart { get; private set;} = new();
        public Subject<int> OnAttackExecute { get; private set;} = new();
        public Subject<int> OnAttackFinish { get; private set;} = new();

        public void OnStep(int legIndex)
        {
            OnFootstep?.OnNext(legIndex);
        }

        public void OnAttackStarted(int attackType)
        {
            OnAttackStart?.OnNext(attackType);
        }

        public void OnAttackExecuted(int attackType)
        {
            OnAttackExecute?.OnNext(attackType);
        }

        public void OnAttackFinished(int attackType)
        {
            OnAttackFinish?.OnNext(attackType);
        }
    }
}
