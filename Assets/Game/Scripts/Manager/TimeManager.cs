using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Time Settings")]
    public float timeScale = 1.0f;
    public int wakeUpHour = 6; // Player wakes up at 6:00 AM
    public int endHour = 24;  // Day technically ends at 24:00 (Midnight)

    [Header("Current Game Time")]
    public int currentDay = 1;
    public float gameTimeInMinutes; // Tracks total minutes passed since 00:00

    // Events triggered when a new day starts or ends
    public event Action OnDayEnd;
    public event Action OnNewDay;

    private bool isGameTimeFlowing = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Start the day at the specified wakeUpHour
        gameTimeInMinutes = wakeUpHour * 60f;
    }

    private void Update()
    {
        // Advance time only if the game time is set to flow
        if (isGameTimeFlowing)
        {
            // Advance time
            gameTimeInMinutes += Time.deltaTime * (60f / timeScale);

            // Check if a new day has started (passed midnight)
            if (gameTimeInMinutes >= endHour * 60f)
            {
                EndDay();
            }
        }
    }

    // --- TIME UTILITIES ---

    public int GetTimeHour()
    {
        return Mathf.FloorToInt(gameTimeInMinutes / 60f) % 24;
    }

    public int GetTimeMinute()
    {
        return Mathf.FloorToInt(gameTimeInMinutes % 60f);
    }

    // --- SLEEP & DAY CYCLE MANAGEMENT ---

    /// <summary>
    /// Pauses time and initiates the sleep sequence.
    /// </summary>
    public void StartSleep()
    {
        isGameTimeFlowing = false;
        // TO DO: Open the Sleep UI (where player selects wake-up hour)
        Debug.Log("Time paused. Sleep UI opened.");
    }

    /// <summary>
    /// Forces the end of the day and starts the next day at the specified hour.
    /// </summary>
    /// <param name="targetHour">The hour the player wakes up (e.g., 6, 8, 10).</param>
    public void WakeUp(int targetHour)
    {
        // Prevent waking up before the day officially ends if it's already nighttime
        if (GetTimeHour() < endHour)
        {
            EndDay(targetHour);
        }
        else
        {
            // If already past midnight, just start the new day at target hour
            StartNewDay(targetHour);
        }
    }

    private void EndDay(int wakeHour = -1)
    {
        // End day logic (Saving, status update)
        Debug.Log($"Day {currentDay} ended due to Sleep/Midnight.");

        // 1. Invoke the End Day event for subscribers (FarmingManager)
        OnDayEnd?.Invoke();

        // 2. Start the new day after a short transition
        int nextWakeHour = (wakeHour == -1) ? wakeUpHour : wakeHour; // Use default or selected hour
        Invoke("StartNewDayTransition", 2f); // Short delay before day starts

        // Use a temp variable to pass the selected hour to the transition function
        PlayerPrefs.SetInt("NextWakeHour", nextWakeHour);
    }

    private void StartNewDayTransition()
    {
        int nextWakeHour = PlayerPrefs.GetInt("NextWakeHour", wakeUpHour);
        StartNewDay(nextWakeHour);
    }

    private void StartNewDay(int startHour)
    {
        currentDay++;
        // Reset time to the selected hour
        gameTimeInMinutes = startHour * 60f;
        isGameTimeFlowing = true;

        Debug.Log($"Starting Day {currentDay}! Waking up at {startHour}:00.");
        OnNewDay?.Invoke();
    }
}