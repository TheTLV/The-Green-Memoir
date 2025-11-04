using UnityEngine;
using TMPro; // Important for TextMeshPro
using UnityEngine.UI; // For Image (Weather Icon)

public class HUDManager : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI dayText;   // For "18 Monday"
    [SerializeField] private TextMeshProUGUI timeText;  // For "13:11"
    [SerializeField] private TextMeshProUGUI moneyText; // For "9721450"

    [Header("Icon References")]
    [SerializeField] private Image timeIcon; // For Sun/Moon icon
    [SerializeField] private Sprite sunIcon;
    [SerializeField] private Sprite moonIcon;

    // Day names array for easy conversion (Optional)
    private string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

    // Update is called once per frame
    private void Update()
    {
        // 1. Update Time and Day (Needs TimeManager)
        UpdateTimeDisplay();

        // 2. Update Money (Needs InventoryManager or separate MoneyManager)
        UpdateMoneyDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (TimeManager.Instance == null) return;

        // Get current time from the Manager
        int currentHour = TimeManager.Instance.GetTimeHour();
        int currentMinute = TimeManager.Instance.GetTimeMinute();
        int currentDay = TimeManager.Instance.currentDay;

        // --- 1. Day Text (e.g., "18 Monday") ---
        // We use Modulo 7 to cycle through days of the week
        string currentDayName = dayNames[(currentDay - 1) % 7];
        dayText.text = $"{currentDay} {currentDayName}";

        // --- 2. Time Text (e.g., "13:11") ---
        // Use ToString("D2") to pad with leading zero (e.g., 09:05)
        timeText.text = $"{currentHour:D2}:{currentMinute:D2}";

        // --- 3. Time Icon (Sun/Moon) ---
        // Change icon based on time of day (e.g., 6:00 to 18:00 is Day)
        if (timeIcon != null)
        {
            if (currentHour >= 6 && currentHour < 18) // Day time
            {
                timeIcon.sprite = sunIcon;
            }
            else // Night time
            {
                timeIcon.sprite = moonIcon;
            }
        }
    }

    private void UpdateMoneyDisplay()
    {
        // 1. Get Money Value (Assuming InventoryManager holds the money amount)
        // If you have a dedicated MoneyManager, use that instead.
        if (InventoryManager.Instance == null) return;

        // TO DO: Define a public long moneyAmount in InventoryManager.cs
        long currentMoney = InventoryManager.Instance.moneyAmount;

        // 2. Format and display
        // Example: Use string formatting to separate thousands (Optional)
        moneyText.text = currentMoney.ToString("N0"); // "N0" formats with thousand separators
    }
}