using System.Linq;
using CombatArena.Game.Gameplay.HealthSystem;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIDamageInfo : MonoBehaviour
    {
        [SerializeField] private UIDamageInfoAnimator m_animator;
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private Image m_statusIcon;
        [Header("Colors")]
        [SerializeField] private Color m_defaultColor;
        [SerializeField] private Color m_criticalColor;
        [SerializeField] private Color m_defendedColor;
        [SerializeField] private Color m_armorBreakColor;
        [Header("Icons")]
        [SerializeField] private Sprite m_armorBreakIcon;
        [SerializeField] private Sprite m_defendedIcon;
        [Header("Extra")]
        [SerializeField] private GameObject m_criticalIndicator;

        public Observable<UIDamageInfo> Show(Damage damage)
        {
            m_text.text = damage.ResultValue.ToString();

            m_criticalIndicator.SetActive(damage.IsCritical);

            var color = damage.IsCritical ? m_criticalColor : m_defaultColor;
            m_text.color = color;
            m_statusIcon.color = color;

            if (damage.Type == DamageType.ArmorBreak)
            {
                m_statusIcon.sprite = m_armorBreakIcon;
                m_statusIcon.color = m_armorBreakColor;
                if (!damage.IsCritical) m_text.color = m_armorBreakColor;
            }
            else
            {
                var defenseMod = damage.Modifiers.FirstOrDefault(v => v is ArmorDefenceModifier);

                if (defenseMod == null)
                {
                    m_statusIcon.enabled = false;
                    
                    m_animator.Play(this);

                    return m_animator.OnEnd;
                }
                else
                {
                    if (!damage.IsCritical) m_text.color = m_defendedColor;
                    m_statusIcon.sprite = m_defendedIcon;
                    m_statusIcon.color = m_defendedColor;
                }
            }

            m_statusIcon.enabled = true;

            m_animator.Play(this);

            return m_animator.OnEnd;
        }

        public void ResetState() => m_animator.ResetState();
    }
}
