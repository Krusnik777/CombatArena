using System;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace CombatArena.Game.Gameplay.Entities.Enemy
{
    public class TargetPursuer : IDisposable
    {
        private Transform _pursueTarget;
        private NavMeshAgent _agent;
        private float _movementSpeed;
        private float _stoppingDistance;

        private Action _onReach;

        private IDisposable _pursueUpdateDisposable;

        public TargetPursuer(Transform target, NavMeshAgent agent, float movementSpeed, float stoppingDistance)
        {
            _pursueTarget = target;
            _agent = agent;
            _movementSpeed = movementSpeed;
            _stoppingDistance = stoppingDistance;
        }

        public void Dispose()
        {
            StopPursue();
        }

        public void SetActionOnReach(Action onReach)
        {
            _onReach = onReach;
        }

        public void StartPursue()
        {
            _agent.speed = _movementSpeed;

            _pursueUpdateDisposable = Observable.EveryUpdate().Subscribe(_ => UpdatePursue());
        }

        public void StopPursue()
        {
            _pursueUpdateDisposable?.Dispose();

            _agent.speed = 0;
            _agent.destination = _agent.transform.position;
        }

        private void UpdatePursue()
        {
            if (_pursueTarget == null)
            {
                StopPursue();
                return;
            }

            if (Vector3.Distance(_agent.transform.position, _pursueTarget.position) <= _stoppingDistance)
            {
                _onReach?.Invoke();

                return;
            }

            _agent.destination = _pursueTarget.position;
        }
    }
}
