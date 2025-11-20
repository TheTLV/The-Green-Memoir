using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Pixel Art Menu Controller - Quản lý menu theo phong cách pixel art
    /// Tương thích với UIStyleSO - Nếu có UIStyleSO sẽ dùng SO, nếu không dùng giá trị mặc định
    /// </summary>
    public class PixelArtMenuController : MonoBehaviour
    {
        [Header("UI Style (Optional - Dùng SO thay vì hardcode)")]
        [Tooltip("Kéo UIStyleSO vào đây. Nếu để trống, sẽ dùng giá trị mặc định bên dưới")]
        [SerializeField] private UIStyleSO uiStyle;

        [Header("Menu Buttons")]
        [Tooltip("Các nút menu (có thể để trống nếu không dùng)")]
        public Button resumeButton;
        public Button restartButton;
        public Button settingsButton;
        public Button levelsButton;
        public Button inventoryButton;
        public Button equipmentButton;
        public Button shopButton;
        public Button craftButton;
        public Button quitButton;
        public Button playButton;
        public Button loadButton;

        [Header("Button Style Settings (Fallback - dùng nếu không có UIStyleSO)")]
        [Tooltip("Màu nút bình thường (xanh lá nhạt)")]
        public Color buttonNormalColor = new Color(0.4f, 0.7f, 0.4f); // #66B266
        
        [Tooltip("Màu nút khi highlight")]
        public Color buttonHighlightedColor = new Color(0.48f, 0.77f, 0.48f); // #7BC47B
        
        [Tooltip("Màu nút khi press")]
        public Color buttonPressedColor = new Color(0.29f, 0.56f, 0.29f); // #4A8F4A
        
        [Tooltip("Màu chữ trên nút (xanh đậm)")]
        public Color buttonTextColor = new Color(0.2f, 0.3f, 0.2f); // #336633
        
        [Tooltip("Màu nền menu (xanh đậm)")]
        public Color backgroundColor = new Color(0.05f, 0.17f, 0.05f); // #0D2B0D
        
        [Tooltip("Màu viền/shadow (be/nâu nhạt)")]
        public Color borderColor = new Color(0.82f, 0.71f, 0.55f); // #D2B48C

        [Header("Font Settings (Fallback - dùng nếu không có UIStyleSO)")]
        [Tooltip("Font pixel art (Press Start 2P hoặc font tương tự)")]
        public TMP_FontAsset pixelArtFont;
        
        [Tooltip("Font size cho text trên nút")]
        public int fontSize = 28;
        
        [Tooltip("Khoảng cách giữa các ký tự")]
        public float characterSpacing = 3f;

        private UIStyleApplier styleApplier;

        void Start()
        {
            // Tự động tìm UIStyleApplier hoặc tạo mới
            styleApplier = GetComponent<UIStyleApplier>();
            if (styleApplier == null)
            {
                styleApplier = gameObject.AddComponent<UIStyleApplier>();
            }

            // Nếu có UIStyleSO, set vào UIStyleApplier
            if (uiStyle != null)
            {
                styleApplier.SetStyle(uiStyle);
            }

            SetupButtons();
            
            // Apply style (từ SO hoặc fallback)
            ApplyButtonStyles();
        }

        void SetupButtons()
        {
            // Setup từng nút
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResume);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestart);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettings);
            
            if (levelsButton != null)
                levelsButton.onClick.AddListener(OnLevels);
            
            if (inventoryButton != null)
                inventoryButton.onClick.AddListener(OnInventory);
            
            if (equipmentButton != null)
                equipmentButton.onClick.AddListener(OnEquipment);
            
            if (shopButton != null)
                shopButton.onClick.AddListener(OnShop);
            
            if (craftButton != null)
                craftButton.onClick.AddListener(OnCraft);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
            
            if (playButton != null)
                playButton.onClick.AddListener(OnPlay);
            
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoad);
        }

        void ApplyButtonStyles()
        {
            // Nếu có UIStyleSO, UIStyleApplier đã tự động apply rồi
            if (uiStyle != null)
            {
                return; // Không cần làm gì thêm
            }

            // Fallback: Áp dụng style thủ công (giữ tương thích với code cũ)
            Button[] allButtons = GetComponentsInChildren<Button>();
            
            foreach (Button btn in allButtons)
            {
                // Set màu nút
                ColorBlock colors = btn.colors;
                colors.normalColor = buttonNormalColor;
                colors.highlightedColor = buttonHighlightedColor;
                colors.pressedColor = buttonPressedColor;
                colors.selectedColor = buttonNormalColor;
                btn.colors = colors;

                // Set font và màu chữ
                var text = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    if (pixelArtFont != null)
                        text.font = pixelArtFont;
                    
                    text.color = buttonTextColor;
                    text.fontSize = fontSize;
                    text.characterSpacing = characterSpacing;
                    text.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    // Fallback: Dùng Text thông thường
                    var textLegacy = btn.GetComponentInChildren<Text>();
                    if (textLegacy != null)
                    {
                        textLegacy.color = buttonTextColor;
                        textLegacy.fontSize = fontSize;
                        textLegacy.alignment = TextAnchor.MiddleCenter;
                    }
                }

                // Thêm shadow/border effect
                AddButtonShadow(btn);
            }
        }

        void AddButtonShadow(Button button)
        {
            // Thêm Shadow component để tạo hiệu ứng 3D
            var shadow = button.GetComponent<UnityEngine.UI.Shadow>();
            if (shadow == null)
            {
                shadow = button.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            }
            
            shadow.effectColor = borderColor;
            shadow.effectDistance = new Vector2(2, -2); // Shadow phía dưới bên phải
            shadow.useGraphicAlpha = true;
        }

        // Button handlers
        public void OnResume() 
        { 
            Debug.Log("Resume clicked");
            // TODO: Resume game logic
        }
        
        public void OnRestart() 
        { 
            Debug.Log("Restart clicked");
            // TODO: Restart game logic
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        
        public void OnSettings() 
        { 
            Debug.Log("Settings clicked");
            // TODO: Open settings menu
        }
        
        public void OnLevels() 
        { 
            Debug.Log("Levels clicked");
            // TODO: Open levels menu
        }
        
        public void OnInventory() 
        { 
            Debug.Log("Inventory clicked");
            // TODO: Open inventory
        }
        
        public void OnEquipment() 
        { 
            Debug.Log("Equipment clicked");
            // TODO: Open equipment menu
        }
        
        public void OnShop() 
        { 
            Debug.Log("Shop clicked");
            // TODO: Open shop
        }
        
        public void OnCraft() 
        { 
            Debug.Log("Craft clicked");
            // TODO: Open crafting menu
        }
        
        public void OnQuit() 
        { 
            Debug.Log("Quit clicked");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        public void OnPlay() 
        { 
            Debug.Log("Play clicked");
            // TODO: Start new game
            var sceneLoader = Managers.SceneLoader.Instance;
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Game");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
        }
        
        public void OnLoad() 
        { 
            Debug.Log("Load clicked");
            // TODO: Load game
            var saveLoadManager = SaveLoad.SaveLoadManager.Instance;
            if (saveLoadManager != null)
            {
                saveLoadManager.LoadGame();
            }
        }
    }
}

