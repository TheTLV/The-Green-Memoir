using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Game Settings Data - Lưu tất cả cài đặt game
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettingsData", menuName = "Game/Settings Data", order = 0)]
    public class GameSettingsDataSO : ScriptableObject
    {
        [Header("Screen Settings")]
        [Tooltip("Full screen mode")]
        public bool isFullScreen = true;

        [Header("Volume Settings (0-100)")]
        [Range(0, 100)]
        [Tooltip("Background Music volume")]
        public int bgmVolume = 100;
        
        [Range(0, 100)]
        [Tooltip("Sound Effects volume")]
        public int seVolume = 100;
        
        [Range(0, 100)]
        [Tooltip("Game Voices volume")]
        public int gameVoicesVolume = 100;
        
        [Range(0, 100)]
        [Tooltip("Event Voices volume")]
        public int eventVoicesVolume = 100;

        [Header("Event Scenes Settings")]
        [Tooltip("Text speed for dialogue")]
        public TextSpeed textSpeed = TextSpeed.Normal;
        
        [Tooltip("Auto mode text speed")]
        public AutoTextSpeed autoModeTextSpeed = AutoTextSpeed.Off;
        
        [Tooltip("Continue playing voices until next voice")]
        public bool continuePlayingVoices = true;

        [Header("Language")]
        [Tooltip("Current language")]
        public Language currentLanguage = Language.English;

        /// <summary>
        /// Text speed options
        /// </summary>
        public enum TextSpeed
        {
            Slow,
            Normal,
            Fast,
            NoWait
        }

        /// <summary>
        /// Auto mode text speed options
        /// </summary>
        public enum AutoTextSpeed
        {
            Off,
            Slow,
            Normal,
            Fast
        }

        /// <summary>
        /// Language options
        /// </summary>
        public enum Language
        {
            English,
            Vietnamese,
            Japanese,
            Chinese,
            Korean
        }

        /// <summary>
        /// Get volume as float (0-1)
        /// </summary>
        public float GetBGMVolumeFloat() => bgmVolume / 100f;
        public float GetSEVolumeFloat() => seVolume / 100f;
        public float GetGameVoicesVolumeFloat() => gameVoicesVolume / 100f;
        public float GetEventVoicesVolumeFloat() => eventVoicesVolume / 100f;

        /// <summary>
        /// Set volume from float (0-1)
        /// </summary>
        public void SetBGMVolume(float volume) => bgmVolume = Mathf.RoundToInt(volume * 100);
        public void SetSEVolume(float volume) => seVolume = Mathf.RoundToInt(volume * 100);
        public void SetGameVoicesVolume(float volume) => gameVoicesVolume = Mathf.RoundToInt(volume * 100);
        public void SetEventVoicesVolume(float volume) => eventVoicesVolume = Mathf.RoundToInt(volume * 100);

        /// <summary>
        /// Reset to default values
        /// </summary>
        public void ResetToDefaults()
        {
            isFullScreen = true;
            bgmVolume = 100;
            seVolume = 100;
            gameVoicesVolume = 100;
            eventVoicesVolume = 100;
            textSpeed = TextSpeed.Normal;
            autoModeTextSpeed = AutoTextSpeed.Off;
            continuePlayingVoices = true;
            currentLanguage = Language.English;
        }
    }
}

