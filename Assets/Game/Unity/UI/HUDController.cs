using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Unity.Presentation.UI;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// HUD Controller - Quản lý UI ngoài game (Player Info, Time, Money, Inventory Button)
    /// Flexible: Không lỗi nếu thiếu components
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Player Info (Optional)")]
        [SerializeField] private Image playerIcon;
        [SerializeField] private TextMeshProUGUI playerNameText;

        [Header("Time/Date (Optional)")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dateText;

        [Header("Money (Optional)")]
        [SerializeField] private Image moneyIcon;
        [SerializeField] private TextMeshProUGUI moneyAmountText;

        [Header("Inventory Button (Optional)")]
        [SerializeField] private UnityEngine.UI.Button inventoryButton;

        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;
        [SerializeField] private float updateInterval = 0.5f; // Update mỗi 0.5 giây

        private float lastUpdateTime = 0f;
        private InventoryUIController inventoryUIController;

        private void Start()
        {
            // Tự động tìm InventoryUIController
            if (inventoryUIController == null)
                inventoryUIController = FindFirstObjectByType<InventoryUIController>();

            // Setup inventory button
            if (inventoryButton != null)
            {
                inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            }

            // Initial update
            UpdateHUD();
        }

        private void Update()
        {
            // Update định kỳ
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateHUD();
                lastUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Update tất cả HUD elements
        /// </summary>
        private void UpdateHUD()
        {
            UpdatePlayerInfo();
            UpdateTimeDate();
            UpdateMoney();
        }

        /// <summary>
        /// Update Player Info
        /// </summary>
        private void UpdatePlayerInfo()
        {
            if (playerNameText != null && GameManager.PlayerService != null)
            {
                var player = GameManager.PlayerService.GetPlayer(playerId);
                if (player != null)
                {
                    // Player không có Name property, dùng Id hoặc "Player"
                    playerNameText.text = player.Id.ToString() ?? "Player";
                }
            }

            // Player icon sẽ được set trong Inspector
        }

        /// <summary>
        /// Update Time/Date
        /// </summary>
        private void UpdateTimeDate()
        {
            if (GameManager.TimeService == null) return;

            if (timeText != null)
            {
                // Format time (ví dụ: "12:00 PM")
                int hour = GameManager.TimeService.CurrentHour;
                int minute = GameManager.TimeService.CurrentMinute;
                string period = hour >= 12 ? "PM" : "AM";
                int displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
                timeText.text = $"{displayHour:00}:{minute:00} {period}";
            }

            if (dateText != null)
            {
                // Format date (ví dụ: "Day 1, Spring")
                int day = GameManager.TimeService.CurrentDay;
                // Tính season từ day (mỗi season = 28 ngày)
                string season = GetSeasonFromDay(day);
                dateText.text = $"Day {day}, {season}";
            }
        }

        /// <summary>
        /// Tính season từ day (mỗi season = 28 ngày)
        /// </summary>
        private string GetSeasonFromDay(int day)
        {
            int seasonIndex = ((day - 1) / 28) % 4;
            switch (seasonIndex)
            {
                case 0: return "Spring";
                case 1: return "Summer";
                case 2: return "Fall";
                case 3: return "Winter";
                default: return "Spring";
            }
        }

        /// <summary>
        /// Update Money
        /// </summary>
        private void UpdateMoney()
        {
            if (moneyAmountText != null && GameManager.PlayerService != null)
            {
                var player = GameManager.PlayerService.GetPlayer(playerId);
                if (player != null)
                {
                    // Money là struct, không thể null, dùng Amount trực tiếp
                    moneyAmountText.text = player.Money.Amount.ToString();
                }
            }

            // Money icon sẽ được set trong Inspector
        }

        /// <summary>
        /// Inventory button clicked
        /// </summary>
        private void OnInventoryButtonClicked()
        {
            if (inventoryUIController != null)
            {
                inventoryUIController.ToggleUI();
            }
            else
            {
                Debug.LogWarning("InventoryUIController not found!");
            }
        }
    }
}

