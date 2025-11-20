using UnityEngine;

namespace TheGreenMemoir.Unity.Audio
{
    /// <summary>
    /// ScriptableObject chứa AudioClip và settings
    /// Dùng để gán vào các ActionSO để tự động phát âm thanh
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClip", menuName = "Game/Audio/Audio Clip", order = 50)]
    public class AudioClipSO : ScriptableObject
    {
        [Header("Audio Clip")]
        [Tooltip("Audio clip để phát")]
        public AudioClip clip;

        [Header("Settings")]
        [Tooltip("Volume (0-1)")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Tooltip("Pitch (-3 đến 3)")]
        [Range(-3f, 3f)]
        public float pitch = 1f;

        [Header("3D Sound (optional)")]
        [Tooltip("Nếu true, phát tại vị trí (cần position)")]
        public bool is3D = false;

        [Tooltip("Min distance cho 3D sound")]
        public float minDistance = 1f;

        [Tooltip("Max distance cho 3D sound")]
        public float maxDistance = 10f;

        /// <summary>
        /// Phát âm thanh
        /// </summary>
        public void Play(Vector3? position = null)
        {
            if (clip == null || AudioManager.Instance == null) return;

            if (is3D && position.HasValue)
            {
                AudioManager.Instance.PlaySFX3D(clip, position.Value, volume, minDistance, maxDistance);
            }
            else
            {
                AudioManager.Instance.PlaySFX(clip, volume, pitch, position);
            }
        }
    }
}

