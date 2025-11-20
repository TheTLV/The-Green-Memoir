using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Component để tự động thêm sound cho buttons
    /// Flexible: Không lỗi nếu AudioManager không có
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonSoundHelper : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private bool useDefaultSounds = true;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }

        private void OnButtonClicked()
        {
            if (AudioManager.Instance == null) return;

            if (useDefaultSounds)
            {
                AudioManager.Instance.PlayButtonClick();
            }
            else if (clickSound != null)
            {
                AudioManager.Instance.PlaySFX(clickSound, 0.5f);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AudioManager.Instance == null) return;

            if (useDefaultSounds)
            {
                AudioManager.Instance.PlayButtonHover();
            }
            else if (hoverSound != null)
            {
                AudioManager.Instance.PlaySFX(hoverSound, 0.3f);
            }
        }
    }
}

