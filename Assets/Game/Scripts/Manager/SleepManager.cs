using UnityEngine;
using UnityEngine.UI;

public class SleepController : MonoBehaviour
{
    // Make sure the Sleep UI Panel is assigned to this GameObject and is initially deactivated.

    [Header("UI References")]
    // Assign your Sleep/Bed Button here
    public Button sleepButton;
    // Assign your UI buttons for selecting wake-up time (e.g., 6 AM Button)
    public Button wakeUp6AMButton;
    public Button wakeUp8AMButton;
    public Button wakeUp10AMButton;

    private void Start()
    {
        // 1. Assign the main sleep action to the button
        sleepButton.onClick.AddListener(() => TimeManager.Instance.StartSleep());

        // 2. Assign wake-up hours to the selection buttons
        wakeUp6AMButton.onClick.AddListener(() => ConfirmWakeUp(6));
        wakeUp8AMButton.onClick.AddListener(() => ConfirmWakeUp(8));
        wakeUp10AMButton.onClick.AddListener(() => ConfirmWakeUp(10));

        // Initial state: Hide the Sleep UI
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the player clicks on a wake-up time button.
    /// </summary>
    public void ConfirmWakeUp(int hour)
    {
        // 1. Call the TimeManager to end the day and wake up at the chosen hour
        TimeManager.Instance.WakeUp(hour);

        // 2. Close the sleep UI
        gameObject.SetActive(false);
    }

    // You can also add a method here to open the Sleep UI when TimeManager.StartSleep() is called.
    public void OpenSleepUI()
    {
        gameObject.SetActive(true);
    }
}