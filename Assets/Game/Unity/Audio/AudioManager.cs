using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Audio
{
    /// <summary>
    /// Quản lý âm thanh trong game (Music, SFX)
    /// Singleton pattern
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance => _instance;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";
        [SerializeField] private string gameVoicesVolumeParam = "GameVoicesVolume";
        [SerializeField] private string eventVoicesVolumeParam = "EventVoicesVolume";
        [SerializeField] private string ambientVolumeParam = "AmbientVolume";

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource gameVoicesSource;
        [SerializeField] private AudioSource eventVoicesSource;
        [SerializeField] private int poolSize = 10; // Số AudioSource trong pool

        [Header("Settings")]
        [SerializeField] private float defaultMusicVolume = 0.7f;
        [SerializeField] private float defaultSFXVolume = 1f;
        [SerializeField] private float defaultGameVoicesVolume = 1f;
        [SerializeField] private float defaultEventVoicesVolume = 1f;
        [SerializeField] private float defaultAmbientVolume = 0.5f;

        [Header("Default Audio Clips (Optional)")]
        [SerializeField] private AudioClip defaultButtonClickSound;
        [SerializeField] private AudioClip defaultButtonHoverSound;
        [SerializeField] private AudioClip defaultAmbientSound;

        private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> _activeSources = new List<AudioSource>();
        private AudioSource ambientSource;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Initialize()
        {
            // Tạo music source nếu chưa có
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Tạo SFX source nếu chưa có
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            // Tạo Game Voices source nếu chưa có
            if (gameVoicesSource == null)
            {
                GameObject gameVoicesObj = new GameObject("GameVoicesSource");
                gameVoicesObj.transform.SetParent(transform);
                gameVoicesSource = gameVoicesObj.AddComponent<AudioSource>();
                gameVoicesSource.playOnAwake = false;
            }

            // Tạo Event Voices source nếu chưa có
            if (eventVoicesSource == null)
            {
                GameObject eventVoicesObj = new GameObject("EventVoicesSource");
                eventVoicesObj.transform.SetParent(transform);
                eventVoicesSource = eventVoicesObj.AddComponent<AudioSource>();
                eventVoicesSource.playOnAwake = false;
            }

            // Tạo Ambient source nếu chưa có
            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }

            // Tạo pool AudioSource cho SFX
            for (int i = 0; i < poolSize; i++)
            {
                GameObject poolObj = new GameObject($"SFXPool_{i}");
                poolObj.transform.SetParent(transform);
                AudioSource source = poolObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                poolObj.SetActive(false);
                _audioSourcePool.Enqueue(source);
            }

            // Load volume settings
            LoadVolumeSettings();

            // Start default ambient sound nếu có
            if (defaultAmbientSound != null)
            {
                PlayAmbient(defaultAmbientSound, defaultAmbientVolume);
            }
        }

        /// <summary>
        /// Phát nhạc nền
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
        {
            if (musicSource == null || clip == null) return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = volume * defaultMusicVolume;
            musicSource.Play();
        }

        /// <summary>
        /// Dừng nhạc nền
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        /// <summary>
        /// Phát SFX (dùng pool để có thể phát nhiều cùng lúc)
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3? position = null)
        {
            if (clip == null) return;

            AudioSource source = GetPooledAudioSource();
            if (source == null)
            {
                // Nếu pool hết, dùng SFX source chính
                source = sfxSource;
            }

            source.clip = clip;
            source.volume = volume * defaultSFXVolume;
            source.pitch = pitch;
            source.spatialBlend = position.HasValue ? 1f : 0f; // 3D nếu có position

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.spatialBlend = 1f;
            }

            source.Play();

            // Nếu dùng pool, trả về sau khi phát xong
            if (source != sfxSource)
            {
                StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
            }
        }

        /// <summary>
        /// Phát SFX 3D (tại vị trí)
        /// </summary>
        public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1f, float minDistance = 1f, float maxDistance = 10f)
        {
            if (clip == null) return;

            AudioSource source = GetPooledAudioSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = volume * defaultSFXVolume;
            source.spatialBlend = 1f;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.transform.position = position;
            source.Play();

            StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
        }

        /// <summary>
        /// Set volume (0-1)
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            SetVolume(masterVolumeParam, volume);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            defaultMusicVolume = volume;
            if (musicSource != null)
                musicSource.volume = volume;
            SetVolume(musicVolumeParam, volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            defaultSFXVolume = volume;
            SetVolume(sfxVolumeParam, volume);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void SetGameVoicesVolume(float volume)
        {
            defaultGameVoicesVolume = volume;
            if (gameVoicesSource != null)
                gameVoicesSource.volume = volume;
            SetVolume(gameVoicesVolumeParam, volume);
            PlayerPrefs.SetFloat("GameVoicesVolume", volume);
        }

        public void SetEventVoicesVolume(float volume)
        {
            defaultEventVoicesVolume = volume;
            if (eventVoicesSource != null)
                eventVoicesSource.volume = volume;
            SetVolume(eventVoicesVolumeParam, volume);
            PlayerPrefs.SetFloat("EventVoicesVolume", volume);
        }

        public void SetAmbientVolume(float volume)
        {
            defaultAmbientVolume = volume;
            if (ambientSource != null)
                ambientSource.volume = volume;
            SetVolume(ambientVolumeParam, volume);
            PlayerPrefs.SetFloat("AmbientVolume", volume);
        }

        private void SetVolume(string paramName, float volume)
        {
            if (audioMixer != null && !string.IsNullOrEmpty(paramName))
            {
                // Chuyển 0-1 thành dB (-80 đến 0)
                float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
                audioMixer.SetFloat(paramName, dB);
            }
        }

        /// <summary>
        /// Load volume settings từ PlayerPrefs
        /// </summary>
        private void LoadVolumeSettings()
        {
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
            float gameVoicesVol = PlayerPrefs.GetFloat("GameVoicesVolume", defaultGameVoicesVolume);
            float eventVoicesVol = PlayerPrefs.GetFloat("EventVoicesVolume", defaultEventVoicesVolume);
            float ambientVol = PlayerPrefs.GetFloat("AmbientVolume", defaultAmbientVolume);

            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);
            SetSFXVolume(sfxVol);
            SetGameVoicesVolume(gameVoicesVol);
            SetEventVoicesVolume(eventVoicesVol);
            SetAmbientVolume(ambientVol);
        }

        /// <summary>
        /// Phát Ambient sound (âm thanh môi trường)
        /// </summary>
        public void PlayAmbient(AudioClip clip, float volume = 1f)
        {
            if (ambientSource == null || clip == null) return;
            ambientSource.clip = clip;
            ambientSource.volume = volume * defaultAmbientVolume;
            ambientSource.Play();
        }

        /// <summary>
        /// Dừng Ambient sound
        /// </summary>
        public void StopAmbient()
        {
            if (ambientSource != null)
                ambientSource.Stop();
        }

        /// <summary>
        /// Phát button click sound (dùng default hoặc custom)
        /// </summary>
        public void PlayButtonClick(AudioClip customSound = null)
        {
            AudioClip soundToPlay = customSound != null ? customSound : defaultButtonClickSound;
            if (soundToPlay != null)
            {
                PlaySFX(soundToPlay, 0.5f);
            }
        }

        /// <summary>
        /// Phát button hover sound (dùng default hoặc custom)
        /// </summary>
        public void PlayButtonHover(AudioClip customSound = null)
        {
            AudioClip soundToPlay = customSound != null ? customSound : defaultButtonHoverSound;
            if (soundToPlay != null)
            {
                PlaySFX(soundToPlay, 0.3f);
            }
        }

        /// <summary>
        /// Phát Game Voice (giọng nói trong gameplay)
        /// </summary>
        public void PlayGameVoice(AudioClip clip, float volume = 1f)
        {
            if (gameVoicesSource == null || clip == null) return;
            gameVoicesSource.clip = clip;
            gameVoicesSource.volume = volume * defaultGameVoicesVolume;
            gameVoicesSource.Play();
        }

        /// <summary>
        /// Phát Event Voice (giọng nói trong event scenes)
        /// </summary>
        public void PlayEventVoice(AudioClip clip, float volume = 1f)
        {
            if (eventVoicesSource == null || clip == null) return;
            eventVoicesSource.clip = clip;
            eventVoicesSource.volume = volume * defaultEventVoicesVolume;
            eventVoicesSource.Play();
        }

        private AudioSource GetPooledAudioSource()
        {
            if (_audioSourcePool.Count > 0)
            {
                AudioSource source = _audioSourcePool.Dequeue();
                source.gameObject.SetActive(true);
                _activeSources.Add(source);
                return source;
            }
            return null;
        }

        private System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
                _activeSources.Remove(source);
                _audioSourcePool.Enqueue(source);
            }
        }
    }
}

