using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using CombatArena.Game.Services;

namespace CombatArena.Game.Gameplay.UI
{
    public class UIControlsTip : MonoBehaviour, System.IDisposable
    {
        [Header("Text")]
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_keyboardTip;
        [SerializeField] private string m_gamepadTip;
        [Header("Image")]
        [SerializeField] private Image m_image;
        [SerializeField] private Sprite m_keyboardButtonImage;
        [SerializeField] private Sprite m_gamepadButtonImage;

        private System.IDisposable disposable;

        public void Initialize()
        {
            disposable?.Dispose();

            disposable = InputDeviceDetectService.CurrentControlDevice.Subscribe(ChangeTipByDevice);
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }

        private void ChangeTipByDevice(UnityEngine.InputSystem.InputDevice device)
        {
            if (device is UnityEngine.InputSystem.Gamepad)
            {
                SetTip(m_gamepadTip, m_gamepadButtonImage);
            }
            else
            {
                SetTip(m_keyboardTip, m_keyboardButtonImage);
            }
        }

        private void SetTip(string tip, Sprite sprite)
        {
            if (m_text != null) m_text.text = tip;
            if (m_image != null)
            {
                m_image.enabled = sprite != null;
                m_image.sprite = sprite;
            }
        }
    }
}
