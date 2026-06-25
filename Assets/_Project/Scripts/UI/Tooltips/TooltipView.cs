using TMPro;
using UnityEngine;

namespace UI.Tooltips
{
    public class TooltipView : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private TMP_Text m_info;

        public void Setup(TooltipData data)
        {
            m_title.text = data.Title;
            if (m_description != null) m_description.text = data.Description;
            if (m_info != null) m_info.text = data.Info;
        }
    }
}
