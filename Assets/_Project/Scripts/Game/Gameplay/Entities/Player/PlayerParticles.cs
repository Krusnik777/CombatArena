using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerParticles : MonoBehaviour
    {
        [SerializeField] private Transform m_particlesPoolParent;
        [SerializeField] private AnimatorEventsCollector m_eventsCollector;
        [Header("Footsteps")]
        [SerializeField] private GameObject m_stepParticlePrefab;
        [SerializeField] private Transform m_leftFootstep;
        [SerializeField] private Transform m_rightFootstep;
        [Header("Attack")]
        [SerializeField] private GameObject m_attackParticlePrefab;
        [SerializeField] private GameObject m_earthBreakParticlePrefab;

        private SimpleGameObjectsPool _particlesPool;

        private CompositeDisposable _disposables;
        
        private void Awake()
        {
            _disposables?.Dispose();

            _particlesPool = new (m_particlesPoolParent, m_stepParticlePrefab, m_attackParticlePrefab, m_earthBreakParticlePrefab);

            _disposables = new()
            {
                m_eventsCollector.OnFootstep.Subscribe(OnStep),
                m_eventsCollector.OnAttackStart.Subscribe(OnAttackStart),
                m_eventsCollector.OnAttackExecute.Subscribe(OnAttackExecute)
            };
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
            _particlesPool?.Dispose();
        }

        private void OnStep(int legIndex)
        {
            var target = legIndex == 0 ? m_leftFootstep : m_rightFootstep;

            var particle = _particlesPool.GetParticle(m_stepParticlePrefab);
            particle.transform.position = target.position;
            _particlesPool.ReturnParticle(particle, 1f);
        }

        private void OnAttackStart(int attackType)
        {
            if (attackType == 0)
            {
                var particle = _particlesPool.GetParticle(m_attackParticlePrefab);
                particle.transform.position = transform.position;
                particle.transform.rotation = transform.localRotation;
                _particlesPool.ReturnParticle(particle, 2f);
            }
        }

        private void OnAttackExecute(int attackType)
        {
            if (attackType == 1)
            {
                var particle = _particlesPool.GetParticle(m_earthBreakParticlePrefab);
                particle.transform.position = transform.position;
                particle.transform.rotation = transform.localRotation;
                _particlesPool.ReturnParticle(particle, 3f);
            }
        }
    }
}
