using DG.Tweening;
using UnityEngine;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class DashEffects : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] m_particles;
        [SerializeField] private bool m_showClone = true;
        [SerializeField] private GameObject m_viewModel;
        [SerializeField] private GameObject m_sword;
        [SerializeField] private Material m_cloneMaterial;

        public void Show()
        {
            if (m_showClone) ShowClone();

            m_viewModel.SetActive(false);
            SetParticlesActive(true);
        }

        public void Hide()
        {
            SetParticlesActive(false);
            m_viewModel.SetActive(true);
        }

        private void ShowClone()
        {
            m_sword.SetActive(false);

            var clone = Instantiate(m_viewModel, transform.position, m_viewModel.transform.localRotation);
            Destroy(clone.GetComponent<PlayerParticles>());
            Destroy(clone.GetComponent<AnimatorEventsCollector>());
            Destroy(clone.GetComponent<Animator>());

            foreach (var mesh in clone.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                mesh.material = m_cloneMaterial;
                mesh.material.DOFloat(2, "_AlphaThreshold", 5f).OnComplete(() => clone.SetActive(false)).SetLink(gameObject);
            }

            m_sword.SetActive(true);
        }

        private void SetParticlesActive(bool state)
        {
            for (int i = 0; i < m_particles.Length; i++)
            {
                if (state) m_particles[i].Play();
                else m_particles[i].Stop();
            }
        }
    }
}
