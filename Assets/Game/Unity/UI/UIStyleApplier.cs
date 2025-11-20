using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// UI Style Applier - Tự động áp dụng UIStyleSO cho tất cả UI elements trong Canvas
    /// Đọc từ UIStyleSO và apply cho Buttons, Text, Images
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class UIStyleApplier : MonoBehaviour
    {
        [Header("UI Style SO")]
        [Tooltip("Kéo UIStyleSO vào đây. Nếu để trống, sẽ tự động tìm trong project")]
        [SerializeField] private UIStyleSO uiStyle;

        [Header("Settings")]
        [Tooltip("Tự động apply khi Start")]
        [SerializeField] private bool applyOnStart = true;
        
        [Tooltip("Tự động apply khi SO thay đổi (chỉ trong Editor)")]
        [SerializeField] private bool applyOnValidate = true;
        
        [Tooltip("Apply cho tất cả child Canvas")]
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
        [SerializeField] private bool applyToChildCanvases = true;
#pragma warning restore CS0414

        [Header("Target Elements (Optional - để trống để tự động tìm)")]
        [Tooltip("Chỉ apply cho các buttons này (để trống = tất cả)")]
        [SerializeField] private Button[] targetButtons;
        
        [Tooltip("Chỉ apply cho các text này (để trống = tất cả)")]
        [SerializeField] private TextMeshProUGUI[] targetTexts;
        
        [Tooltip("Chỉ apply cho các images này (để trống = tất cả)")]
        [SerializeField] private Image[] targetImages;

        private Canvas canvas;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            
            // Tự động tìm UIStyleSO nếu chưa gán
            if (uiStyle == null)
            {
                LoadUIStyleFromResources();
            }
        }

        void Start()
        {
            if (applyOnStart && uiStyle != null)
            {
                ApplyStyle();
            }
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            // Kiểm tra null để tránh lỗi SerializedObjectNotCreatableException
            // Chỉ chạy khi object còn tồn tại và đang trong Play mode
            if (this == null) return;
            try
            {
                if (gameObject == null) return;
            }
            catch (System.Exception)
            {
                // Object đã bị destroy, bỏ qua
                return;
            }
            
            // Chỉ apply khi đang chơi game, không apply trong Editor mode
            if (applyOnValidate && uiStyle != null && Application.isPlaying)
            {
                ApplyStyle();
            }
        }
        #endif

        /// <summary>
        /// Tự động tìm UIStyleSO từ Resources hoặc AssetDatabase
        /// </summary>
        private void LoadUIStyleFromResources()
        {
            // Tìm trong Resources
            uiStyle = Resources.Load<UIStyleSO>("UIStyle");
            
            #if UNITY_EDITOR
            // Tìm trong AssetDatabase (Editor only)
            if (uiStyle == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:UIStyleSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    uiStyle = UnityEditor.AssetDatabase.LoadAssetAtPath<UIStyleSO>(path);
                }
            }
            #endif

            if (uiStyle == null)
            {
                Debug.LogWarning("UIStyleSO not found! UI will use default style. Create one: Project → Create → Game → UI Style");
            }
        }

        /// <summary>
        /// Áp dụng style cho tất cả UI elements
        /// </summary>
        public void ApplyStyle()
        {
            if (uiStyle == null)
            {
                Debug.LogWarning("UIStyleSO is null! Cannot apply style.");
                return;
            }

            ApplyToButtons();
            ApplyToTexts();
            ApplyToImages();
            ApplyToBackground();

            Debug.Log($"UI Style '{uiStyle.styleName}' applied successfully!");
        }

        /// <summary>
        /// Áp dụng style cho tất cả Buttons
        /// </summary>
        private void ApplyToButtons()
        {
            Button[] buttons;
            
            if (targetButtons != null && targetButtons.Length > 0)
            {
                buttons = targetButtons;
            }
            else
            {
                buttons = GetComponentsInChildren<Button>(true);
            }

            foreach (Button btn in buttons)
            {
                if (btn == null) continue;

                // Set màu nút
                btn.colors = uiStyle.GetButtonColorBlock();

                // Set font và màu chữ
                var text = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    ApplyTextStyle(text, uiStyle.buttonTextColor);
                }
                else
                {
                    // Fallback: Dùng Text thông thường
                    var textLegacy = btn.GetComponentInChildren<Text>();
                    if (textLegacy != null)
                    {
                        textLegacy.color = uiStyle.buttonTextColor;
                        textLegacy.fontSize = uiStyle.fontSize;
                        textLegacy.alignment = TextAnchor.MiddleCenter;
                    }
                }

                // Thêm shadow/border
                ApplyButtonEffects(btn);
            }
        }

        /// <summary>
        /// Áp dụng style cho tất cả Texts
        /// </summary>
        private void ApplyToTexts()
        {
            TextMeshProUGUI[] texts;
            
            if (targetTexts != null && targetTexts.Length > 0)
            {
                texts = targetTexts;
            }
            else
            {
                texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            }

            foreach (TextMeshProUGUI text in texts)
            {
                if (text == null) continue;
                
                // Xác định màu chữ (button text hay normal text)
                Color textColor = uiStyle.normalTextColor;
                
                // Nếu text nằm trong button, dùng buttonTextColor
                if (text.GetComponentInParent<Button>() != null)
                {
                    textColor = uiStyle.buttonTextColor;
                }
                // Nếu là title (font size lớn hoặc có tag "Title")
                else if (text.fontSize > uiStyle.fontSize + 10 || text.CompareTag("Title"))
                {
                    textColor = uiStyle.titleTextColor;
                }

                ApplyTextStyle(text, textColor);
            }
        }

        /// <summary>
        /// Áp dụng style cho TextMeshProUGUI
        /// </summary>
        private void ApplyTextStyle(TextMeshProUGUI text, Color color)
        {
            if (text == null) return;

            if (uiStyle.font != null)
                text.font = uiStyle.font;
            
            text.color = color;
            text.fontSize = uiStyle.fontSize;
            text.characterSpacing = uiStyle.characterSpacing;
            text.lineSpacing = uiStyle.lineSpacing;
            text.alignment = TextAlignmentOptions.Center;
        }

        /// <summary>
        /// Áp dụng style cho tất cả Images (background, panels)
        /// </summary>
        private void ApplyToImages()
        {
            Image[] images;
            
            if (targetImages != null && targetImages.Length > 0)
            {
                images = targetImages;
            }
            else
            {
                images = GetComponentsInChildren<Image>(true);
            }

            foreach (Image img in images)
            {
                if (img == null) continue;
                
                // Bỏ qua images trong buttons (đã xử lý ở ApplyToButtons)
                if (img.GetComponentInParent<Button>() != null)
                    continue;

                // Nếu là background
                if (img.CompareTag("Background") || img.name.Contains("Background"))
                {
                    img.color = uiStyle.backgroundColor;
                }
                // Nếu là panel
                else if (img.CompareTag("Panel") || img.name.Contains("Panel"))
                {
                    img.color = uiStyle.panelBackgroundColor;
                }
            }
        }

        /// <summary>
        /// Áp dụng màu nền cho Canvas
        /// </summary>
        private void ApplyToBackground()
        {
            // Tìm background image trong canvas
            Image background = canvas.GetComponentInChildren<Image>();
            if (background != null && (background.CompareTag("Background") || background.name.Contains("Background")))
            {
                background.color = uiStyle.backgroundColor;
            }
        }

        /// <summary>
        /// Áp dụng hiệu ứng cho Button (shadow, border, animation)
        /// </summary>
        private void ApplyButtonEffects(Button button)
        {
            if (button == null) return;

            // Shadow effect
            if (uiStyle.useShadow)
            {
                var shadow = button.GetComponent<UnityEngine.UI.Shadow>();
                if (shadow == null)
                {
                    shadow = button.gameObject.AddComponent<UnityEngine.UI.Shadow>();
                }
                
                shadow.effectColor = uiStyle.shadowColor;
                shadow.effectDistance = uiStyle.shadowDistance;
                shadow.useGraphicAlpha = true;
            }

            // Border effect (optional)
            if (uiStyle.useBorder)
            {
                // Tạo border bằng Outline component
                var outline = button.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = button.gameObject.AddComponent<Outline>();
                }
                
                outline.effectColor = uiStyle.borderColor;
                outline.effectDistance = new Vector2(uiStyle.borderThickness, uiStyle.borderThickness);
            }

            // Hover animation (optional - cần script riêng)
            if (uiStyle.useHoverAnimation)
            {
                var hoverAnim = button.GetComponent<UIButtonHoverAnimation>();
                if (hoverAnim == null)
                {
                    hoverAnim = button.gameObject.AddComponent<UIButtonHoverAnimation>();
                }
                hoverAnim.Setup(uiStyle.hoverScale);
            }

            // Flash effect (optional - cần script riêng)
            if (uiStyle.useFlashEffect)
            {
                var flashEffect = button.GetComponent<UIButtonFlashEffect>();
                if (flashEffect == null)
                {
                    flashEffect = button.gameObject.AddComponent<UIButtonFlashEffect>();
                }
                flashEffect.Setup(uiStyle.flashColor);
            }
        }

        /// <summary>
        /// Set UIStyleSO và apply ngay lập tức
        /// </summary>
        public void SetStyle(UIStyleSO newStyle)
        {
            uiStyle = newStyle;
            ApplyStyle();
        }

        /// <summary>
        /// Get current UIStyleSO
        /// </summary>
        public UIStyleSO GetStyle()
        {
            return uiStyle;
        }
    }
}

