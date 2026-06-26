using System;
using CombatArena.Game.Gameplay.UI;
using R3;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerParticles : MonoBehaviour, IDisposable, IEffectsHolder
    {
        [Header("Footsteps")]
        [SerializeField] private GameObject m_stepParticlePrefab;
        [SerializeField] private Transform m_leftFootstep;
        [SerializeField] private Transform m_rightFootstep;
        [Header("Attacks")]
        [SerializeField] private GameObject m_attackParticlePrefab;
        [SerializeField] private GameObject m_earthBreakParticlePrefab;
        [Header("Health Changes")]
        [SerializeField] private Transform m_hitTransform;
        [SerializeField] private UIDamageInfo m_damageInfoPrefab;
        [SerializeField] private ParticleSystem m_hitEffect;

        public UIDamageInfo UIDamageInfoPrefab => m_damageInfoPrefab;
        public ParticleSystem HitEffect => m_hitEffect;
        public Transform HitTransform => m_hitTransform;

        private AnimatorEventsCollector _eventsCollector;
        private SimpleGameObjectsPool _particlesPool;

        private CompositeDisposable _disposables;

        

        public void Initialize(AnimatorEventsCollector eventsCollector, SimpleGameObjectsPool pool)
        {
            _disposables?.Dispose();

            _eventsCollector = eventsCollector;
            _particlesPool = pool;

            _particlesPool.Add(m_stepParticlePrefab, m_attackParticlePrefab, m_earthBreakParticlePrefab);

            _disposables = new()
            {
                _eventsCollector.OnFootstep.Subscribe(OnStep),
                _eventsCollector.OnAttackStart.Subscribe(OnAttackStart),
                _eventsCollector.OnAttackExecute.Subscribe(OnAttackExecute)
            };
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void OnStep(int legIndex)
        {
            var target = legIndex == 0 ? m_leftFootstep : m_rightFootstep;

            var particle = _particlesPool.Get(m_stepParticlePrefab);
            particle.transform.position = target.position;
            _particlesPool.Return(particle, 1f);
        }

        private void OnAttackStart(int attackType)
        {
            if (attackType == 0)
            {
                var particle = _particlesPool.Get(m_attackParticlePrefab);
                particle.transform.position = transform.position;
                particle.transform.rotation = transform.localRotation;
                _particlesPool.Return(particle, 2f);
            }
        }

        private void OnAttackExecute(int attackType)
        {
            if (attackType == 1)
            {
                var particle = _particlesPool.Get(m_earthBreakParticlePrefab);
                particle.transform.position = transform.position;
                particle.transform.rotation = transform.localRotation;
                _particlesPool.Return(particle, 3f);
            }
        }
    }
}
