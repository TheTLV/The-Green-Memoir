using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Settings Controller - Quản lý màn hình cài đặt
    /// Hỗ trợ Audio và BGM volume với sprite animation
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        [Header("Audio Volume (Dùng Slider hoặc VolumeSliderUI)")]
        [Tooltip("Slider cho Audio volume (nếu dùng slider thông thường)")]
        [SerializeField] private Slider audioVolumeSlider;
        
        [Tooltip("VolumeSliderUI cho Audio (nếu dùng sprite blocks)")]
        [SerializeField] private VolumeSliderUI audioVolumeSliderUI;
        
        [Tooltip("Icon cho Audio (speaker icon từ sprite sheet)")]
        [SerializeField] private Image audioIcon;

        [Header("BGM Volume (Dùng Slider hoặc VolumeSliderUI)")]
        [Tooltip("Slider cho BGM volume (nếu dùng slider thông thường)")]
        [SerializeField] private Slider bgmVolumeSlider;
        
        [Tooltip("VolumeSliderUI cho BGM (nếu dùng sprite blocks)")]
        [SerializeField] private VolumeSliderUI bgmVolumeSliderUI;
        
        [Tooltip("Icon cho BGM (music note icon từ sprite sheet)")]
        [SerializeField] private Image bgmIcon;

        [Header("Audio Mixer (Optional)")]
        [Tooltip("Audio Mixer nếu không dùng AudioManager")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("UI")]
        [Tooltip("Settings Panel GameObject")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Buttons")]
        [Tooltip("Button để đóng settings")]
        [SerializeField] private Button backButton;

        private void Start()
        {
            // Load và setup Audio volume
            float audioVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            SetupAudioVolume(audioVol);

            // Load và setup BGM volume
            float bgmVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            SetupBGMVolume(bgmVol);

            // Link back button
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /// <summary>
        /// Setup Audio volume slider/UI
        /// </summary>
        private void SetupAudioVolume(float volume)
        {
            if (audioVolumeSlider != null)
            {
                audioVolumeSlider.value = volume;
                audioVolumeSlider.onValueChanged.AddListener(OnAudioVolumeChanged);
            }

            if (audioVolumeSliderUI != null)
            {
                audioVolumeSliderUI.UpdateVolume(volume);
            }
        }

        /// <summary>
        /// Setup BGM volume slider/UI
        /// </summary>
        private void SetupBGMVolume(float volume)
        {
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.value = volume;
                bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }

            if (bgmVolumeSliderUI != null)
            {
                bgmVolumeSliderUI.UpdateVolume(volume);
            }
        }

        /// <summary>
        /// Audio volume changed (từ slider)
        /// </summary>
        public void OnAudioVolumeChanged(float value)
        {
            // Update AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(value);
            }
            else if (audioMixer != null)
            {
                float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
                audioMixer.SetFloat("MasterVolume", dB);
            }

            // Update VolumeSliderUI nếu có
            if (audioVolumeSliderUI != null)
            {
                audioVolumeSliderUI.UpdateVolume(value);
            }

            // Save
            PlayerPrefs.SetFloat("MasterVolume", value);
        }

        /// <summary>
        /// BGM volume changed (từ slider)
        /// </summary>
        public void OnBGMVolumeChanged(float value)
        {
            // Update AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(value);
            }
            else if (audioMixer != null)
            {
                float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
                audioMixer.SetFloat("MusicVolume", dB);
            }

            // Update VolumeSliderUI nếu có
            if (bgmVolumeSliderUI != null)
            {
                bgmVolumeSliderUI.UpdateVolume(value);
            }

            // Save
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        /// <summary>
        /// Increase Audio volume (gọi từ button)
        /// </summary>
        public void IncreaseAudioVolume()
        {
            float currentVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float newVol = Mathf.Clamp01(currentVol + 0.1f);
            OnAudioVolumeChanged(newVol);
            
            if (audioVolumeSlider != null)
                audioVolumeSlider.value = newVol;
        }

        /// <summary>
        /// Decrease Audio volume (gọi từ button)
        /// </summary>
        public void DecreaseAudioVolume()
        {
            float currentVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float newVol = Mathf.Clamp01(currentVol - 0.1f);
            OnAudioVolumeChanged(newVol);
            
            if (audioVolumeSlider != null)
                audioVolumeSlider.value = newVol;
        }

        /// <summary>
        /// Increase BGM volume (gọi từ button)
        /// </summary>
        public void IncreaseBGMVolume()
        {
            float currentVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            float newVol = Mathf.Clamp01(currentVol + 0.1f);
            OnBGMVolumeChanged(newVol);
            
            if (bgmVolumeSlider != null)
                bgmVolumeSlider.value = newVol;
        }

        /// <summary>
        /// Decrease BGM volume (gọi từ button)
        /// </summary>
        public void DecreaseBGMVolume()
        {
            float currentVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            float newVol = Mathf.Clamp01(currentVol - 0.1f);
            OnBGMVolumeChanged(newVol);
            
            if (bgmVolumeSlider != null)
                bgmVolumeSlider.value = newVol;
        }

        /// <summary>
        /// Đóng settings panel
        /// </summary>
        public void OnBackClicked()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        /// <summary>
        /// Mở settings panel
        /// </summary>
        public void ShowSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        /// <summary>
        /// Setup Ambient volume (nếu có slider)
        /// </summary>
        public void OnAmbientVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetAmbientVolume(value);
            }
            PlayerPrefs.SetFloat("AmbientVolume", value);
        }

        /// <summary>
        /// Setup SFX volume (nếu có slider)
        /// </summary>
        public void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
            PlayerPrefs.SetFloat("SFXVolume", value);
        }

        /// <summary>
        /// Setup Game Voices volume (nếu có slider)
        /// </summary>
        public void OnGameVoicesVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetGameVoicesVolume(value);
            }
            PlayerPrefs.SetFloat("GameVoicesVolume", value);
        }

        /// <summary>
        /// Setup Event Voices volume (nếu có slider)
        /// </summary>
        public void OnEventVoicesVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetEventVoicesVolume(value);
            }
            PlayerPrefs.SetFloat("EventVoicesVolume", value);
        }
    }
}

