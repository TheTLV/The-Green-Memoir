using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // Nếu đã có 1 AudioManager, xóa cái mới tạo
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Gán instance & giữ lại khi đổi scene
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Lấy AudioSource và phát nhạc
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Tuỳ chọn: thay đổi volume
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    // Tuỳ chọn: tắt / bật nhạc
    public void ToggleMusic(bool enable)
    {
        audioSource.mute = !enable;
    }
}
