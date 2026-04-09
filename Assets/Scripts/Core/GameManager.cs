using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Core game manager handling all game state and mechanics.
/// Manages outputs, world health, passive generation, and upgrades.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private float outputs = 0f;
    [SerializeField] private float outputsPerClick = 1f;
    [SerializeField] private float worldHealth = 100f;

    [Header("Day/Quota System")]
    [SerializeField] private int currentDay = 1; // Always starts at Day 1
    [SerializeField] private int tasksCompleted = 0; // Number of minigames completed
    [SerializeField] private int tasksRequired = 4; // Must complete 4 minigames to advance
    [SerializeField] private float dayDuration = 90f; // 1 minute 30 seconds = 90 seconds

    [Header("Day Timer")]
    [SerializeField] private float dayTimer = 0f; // Counts up from 0 to dayDuration
    [SerializeField] private float startTime = 8f; // Display time starts at 8:00 AM
    [SerializeField] private float endTime = 20f; // Display time ends at 8:00 PM (20:00)

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI outputsText;
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button clickButton;
    [SerializeField] private Button sleepButton;

    [Header("World Degradation Settings")]
    [SerializeField] private float worldHealthDecayPerClick = 0.1f;
    [SerializeField] private float worldHealthDecayPerOutput = 0.005f; // TODO: implement world health decay

    [Header("Upgrade Values")]
    [SerializeField] private float scalingEngineMultiplier = 2f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Always start on Day 1
        currentDay = 1;

        // Setup click button
        if (clickButton != null)
        {
            clickButton.onClick.AddListener(OnClick);
        }

        // Setup sleep button
        if (sleepButton != null)
        {
            sleepButton.onClick.AddListener(OnSleep);
            sleepButton.interactable = false; // Disabled until quota is met
        }

        // Initialize day 1
        StartNewDay();

        UpdateUI();
    }

    private void Update()
    {
        // Clamp world health
        worldHealth = Mathf.Max(0, worldHealth);

        // Progress day timer
        dayTimer += Time.deltaTime;

        // Check if day is over (90 seconds elapsed)
        if (dayTimer >= dayDuration)
        {
            EndDay();
        }

        UpdateUI();
    }

    /// <summary>
    /// Called when day timer reaches 90 seconds
    /// </summary>
    private void EndDay()
    {
        dayTimer = dayDuration; // Clamp timer

        // Check win/lose condition
        if (tasksCompleted >= tasksRequired)
        {
            // WIN - advance to next day
            SystemLog.Instance?.LogMessage($"Day {currentDay} complete! Quota met with {tasksCompleted} tasks.");
            AdvanceToNextDay();
        }
        else
        {
            // LOSE - show ending or retry
            SystemLog.Instance?.LogMessage($"Day {currentDay} failed! Only completed {tasksCompleted}/{tasksRequired} tasks.");

            if (currentDay == 5)
            {
                // Day 5 loss = game over
                if (DayManager.Instance != null)
                {
                    DayManager.Instance.TriggerEnding();
                }
            }
            else
            {
                // Earlier days: retry or show day manager
                if (DayManager.Instance != null)
                {
                    DayManager.Instance.ShowDayFailure(currentDay);
                }
            }
        }
    }

    /// <summary>
    /// Advance to the next day
    /// </summary>
    public void AdvanceToNextDay()
    {
        if (currentDay < 5)
        {
            currentDay++;
            StartNewDay();

            if (DayManager.Instance != null)
            {
                DayManager.Instance.ShowUpgradeModal(currentDay - 1); // Show upgrades for previous day
            }
        }
        else
        {
            // Day 5 complete = game won
            SystemLog.Instance?.LogMessage("All 5 days complete! Game won!");
            if (DayManager.Instance != null)
            {
                DayManager.Instance.TriggerEnding();
            }
        }
    }

    /// <summary>
    /// Called when player sleeps (end of day)
    /// </summary>
    public void OnSleep()
    {
        if (tasksCompleted >= tasksRequired)
        {
            EndDay();
        }
        else
        {
            SystemLog.Instance?.LogMessage("Cannot sleep yet - quota not met!");
        }
    }

    /// <summary>
    /// Called when player clicks the main button
    /// </summary>
    public void OnClick()
    {
        outputs += outputsPerClick;
        worldHealth -= worldHealthDecayPerClick;
        worldHealth = Mathf.Max(0, worldHealth);
    }

    /// <summary>
    /// Add outputs (quota points) - used by minigames and shredder
    /// </summary>
    public void AddOutputs(float amount)
    {
        outputs += amount;
        SystemLog.Instance?.LogMessage($"Added {amount} quota points!");
    }

    /// <summary>
    /// Get current daily quota (tasks completed)
    /// </summary>
    public int GetDailyQuota()
    {
        return tasksRequired;
    }

    /// <summary>
    /// Get current daily points (tasks completed)
    /// </summary>
    public int GetDailyPoints()
    {
        return tasksCompleted;
    }

    /// <summary>
    /// Get current outputs/quota
    /// </summary>
    public float GetOutputs()
    {
        return outputs;
    }

    /// <summary>
    /// Get current day
    /// </summary>
    public int GetCurrentDay()
    {
        return currentDay;
    }

    /// <summary>
    /// Multiply outputs per click
    /// </summary>
    public void UpgradeClickPower()
    {
        outputsPerClick *= scalingEngineMultiplier;
    }

    /// <summary>
    /// Update UI displays
    /// </summary>
    private void UpdateUI()
    {
        if (dayText != null)
        {
            dayText.text = $"Day {currentDay}";
        }

        if (quotaText != null)
        {
            quotaText.text = $"Tasks: {tasksCompleted}/{tasksRequired}";
        }

        if (timeText != null)
        {
            // Calculate current display time (8am to 8pm, mapped to 0-90 seconds)
            float timeProgress = dayTimer / dayDuration; // 0 to 1
            float displayTime = startTime + (endTime - startTime) * timeProgress; // 8 to 20 (8am to 8pm)
            timeText.text = $"Time: {FormatTime(displayTime)}";
        }

        if (outputsText != null)
        {
            float remainingTime = dayDuration - dayTimer;
            outputsText.text = $"Time left: {remainingTime:F1}s";
        }
    }

    /// <summary>
    /// Record a completed minigame task
    /// </summary>
    public void CompleteTask()
    {
        tasksCompleted++;
        SystemLog.Instance?.LogMessage($"Task completed! {tasksCompleted}/{tasksRequired}");

        // Check if quota met
        if (tasksCompleted >= tasksRequired)
        {
            SystemLog.Instance?.LogMessage("Day quota met! You can sleep now.");
        }
    }

    /// <summary>
    /// Degrade world health by a specified amount (used by minigames and other systems)
    /// </summary>
    public void DegradeWorldHealth(float amount)
    {
        worldHealth -= amount;
        worldHealth = Mathf.Max(0, worldHealth);
    }

    /// <summary>
    /// Start a new day - reset timer and task count
    /// </summary>
    private void StartNewDay()
    {
        dayTimer = 0f;
        tasksCompleted = 0;

        SystemLog.Instance?.LogMessage($"Day {currentDay} begins. Complete {tasksRequired} minigames in {dayDuration} seconds!");
    }


    /// <summary>
    /// Format time as HH:MM AM/PM
    /// </summary>
    private string FormatTime(float hours)
    {
        int hour = Mathf.FloorToInt(hours);
        int minute = Mathf.FloorToInt((hours - hour) * 60f);

        string period = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;

        return $"{displayHour}:{minute:D2} {period}";
    }

    // Public getters for other systems
    public float GetWorldHealth() => worldHealth;
    public int GetTasksCompleted() => tasksCompleted;
    public int GetTasksRequired() => tasksRequired;
    public float GetDayTimer() => dayTimer;
    public float GetDayDuration() => dayDuration;
}
