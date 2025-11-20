using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// UI Style SO - Cấu hình style UI (Font, Màu sắc, Hiệu ứng)
    /// Cho phép tạo nhiều theme khác nhau (Pixel, Modern, Fantasy, v.v.)
    /// </summary>
    [CreateAssetMenu(fileName = "UIStyle", menuName = "Game/UI Style", order = 50)]
    public class UIStyleSO : ScriptableObject
    {
        [Header("Style Info")]
        [Tooltip("Tên style (vd: Pixel Art, Modern, Fantasy)")]
        public string styleName = "Pixel Art";
        
        [Tooltip("Mô tả style")]
        [TextArea(2, 4)]
        public string description = "Pixel art style UI";

        [Header("Font Settings")]
        [Tooltip("Font cho TextMeshPro (pixel art font)")]
        public TMP_FontAsset font;
        
        [Tooltip("Font size mặc định")]
        [Range(12, 72)]
        public int fontSize = 28;
        
        [Tooltip("Khoảng cách giữa các ký tự")]
        [Range(0f, 20f)]
        public float characterSpacing = 3f;
        
        [Tooltip("Khoảng cách giữa các dòng")]
        [Range(0.5f, 2f)]
        public float lineSpacing = 1.2f;

        [Header("Button Colors")]
        [Tooltip("Màu nút bình thường")]
        public Color buttonNormalColor = new Color(0.4f, 0.7f, 0.4f); // #66B266
        
        [Tooltip("Màu nút khi highlight")]
        public Color buttonHighlightedColor = new Color(0.48f, 0.77f, 0.48f); // #7BC47B
        
        [Tooltip("Màu nút khi press")]
        public Color buttonPressedColor = new Color(0.29f, 0.56f, 0.29f); // #4A8F4A
        
        [Tooltip("Màu nút khi selected")]
        public Color buttonSelectedColor = new Color(0.4f, 0.7f, 0.4f); // #66B266
        
        [Tooltip("Màu nút khi disabled")]
        public Color buttonDisabledColor = new Color(0.5f, 0.5f, 0.5f); // #808080

        [Header("Text Colors")]
        [Tooltip("Màu chữ trên nút")]
        public Color buttonTextColor = new Color(0.2f, 0.3f, 0.2f); // #336633
        
        [Tooltip("Màu chữ thông thường")]
        public Color normalTextColor = new Color(0.9f, 0.9f, 0.9f); // #E6E6E6
        
        [Tooltip("Màu chữ tiêu đề")]
        public Color titleTextColor = new Color(1f, 1f, 1f); // #FFFFFF

        [Header("Background Colors")]
        [Tooltip("Màu nền menu")]
        public Color backgroundColor = new Color(0.05f, 0.17f, 0.05f); // #0D2B0D
        
        [Tooltip("Màu nền panel")]
        public Color panelBackgroundColor = new Color(0.1f, 0.25f, 0.1f); // #1A401A

        [Header("Shadow/Border Effects")]
        [Tooltip("Màu shadow/border")]
        public Color shadowColor = new Color(0.82f, 0.71f, 0.55f); // #D2B48C
        
        [Tooltip("Khoảng cách shadow (x, y)")]
        public Vector2 shadowDistance = new Vector2(2f, -2f);
        
        [Tooltip("Có dùng shadow không")]
        public bool useShadow = true;

        [Header("Border (Optional)")]
        [Tooltip("Có dùng border không")]
        public bool useBorder = false;
        
        [Tooltip("Màu border")]
        public Color borderColor = new Color(0.9f, 0.83f, 0.69f); // #E8D4B0
        
        [Tooltip("Độ dày border")]
        [Range(1f, 10f)]
        public float borderThickness = 2f;

        [Header("Advanced Effects (Optional)")]
        [Tooltip("Có animation khi hover không")]
        public bool useHoverAnimation = false;
        
        [Tooltip("Scale khi hover (1.0 = không đổi)")]
        [Range(0.9f, 1.2f)]
        public float hoverScale = 1.05f;
        
        [Tooltip("Có flash effect khi click không")]
        public bool useFlashEffect = false;
        
        [Tooltip("Màu flash")]
        public Color flashColor = new Color(1f, 1f, 0.8f); // #FFFFCC

        /// <summary>
        /// Validate style settings
        /// </summary>
        private void OnValidate()
        {
            // Kiểm tra null để tránh lỗi khi SO bị destroy
            if (this == null) return;
            
            // Đảm bảo màu sắc hợp lệ
            if (fontSize < 12) fontSize = 12;
            if (fontSize > 72) fontSize = 72;
            if (characterSpacing < 0) characterSpacing = 0;
            if (lineSpacing < 0.5f) lineSpacing = 0.5f;
            if (borderThickness < 1f) borderThickness = 1f;
            if (hoverScale < 0.9f) hoverScale = 0.9f;
            if (hoverScale > 1.2f) hoverScale = 1.2f;
        }

        /// <summary>
        /// Lấy ColorBlock cho Button
        /// </summary>
        public ColorBlock GetButtonColorBlock()
        {
            ColorBlock colors = new ColorBlock();
            colors.normalColor = buttonNormalColor;
            colors.highlightedColor = buttonHighlightedColor;
            colors.pressedColor = buttonPressedColor;
            colors.selectedColor = buttonSelectedColor;
            colors.disabledColor = buttonDisabledColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            return colors;
        }
    }
}

