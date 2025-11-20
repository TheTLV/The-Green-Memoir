using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Story SO - Cấu hình intro story (chạy chữ)
    /// Tất cả cấu hình qua Inspector, không cần code
    /// </summary>
    [CreateAssetMenu(fileName = "Story", menuName = "Game/Story", order = 33)]
    public class StorySO : ScriptableObject
    {
        [Header("Story Text")]
        [TextArea(5, 10)]
        [Tooltip("Nội dung story hiển thị")]
        public string storyText = "Sau nhiều năm làm việc ở thành phố, bạn quyết định trở về quê hương...\n\n" +
                                 "Nơi đây, bạn sẽ bắt đầu lại từ đầu với một mảnh đất nhỏ...\n\n" +
                                 "Trồng rau, nuôi cá, xây dựng lại cuộc sống của mình...\n\n" +
                                 "Chào mừng đến với The Green Memoir!";

        [Header("Settings")]
        [Tooltip("Tốc độ gõ chữ (giây/chữ)")]
        public float typingSpeed = 0.05f;
        
        [Tooltip("Tốc độ fade out (alpha/giây)")]
        public float fadeSpeed = 0.5f;
        
        [Tooltip("Thời gian đợi sau khi gõ xong (giây)")]
        public float waitAfterTyping = 2f;
        
        [Tooltip("Scene load sau khi story xong")]
        public string nextSceneName = "Game";
        
        [Tooltip("Phím để tua nhanh story (giữ phím để tua)")]
        public KeyCode skipKey = KeyCode.Space;
        
        [Tooltip("Hệ số tốc độ khi tua nhanh (ví dụ: 5 = nhanh gấp 5 lần)")]
        [Range(2f, 20f)]
        public float fastForwardSpeedMultiplier = 5f;
    }
}

