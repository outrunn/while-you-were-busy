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
    [SerializeField] private float outputsPerSecond = 0f;
    [SerializeField] private float worldHealth = 100f;

    [Header("Day/Quota System")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private float dailyQuota = 100f;
    [SerializeField] private float dailyPoints = 0f;
    [SerializeField] private float baseQuota = 100f;
    [SerializeField] private float quotaMultiplier = 1.5f;

    [Header("World Time")]
    [SerializeField] private float currentTime = 6f; // Hours (6.0 = 6:00 AM)
    [SerializeField] private float timeProgressionSpeed = 60f; // Real seconds per in-game hour
    [SerializeField] private float startTime = 6f; // Day starts at 6:00 AM

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI outputsText;
    [SerializeField] private TextMeshProUGUI outputsPerSecondText;
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button clickButton;
    [SerializeField] private Button sleepButton;

    [Header("World Degradation Settings")]
    [SerializeField] private float worldHealthDecayPerClick = 0.1f;
    [SerializeField] private float worldHealthDecayPerOutput = 0.005f;

    [Header("Upgrade Costs")]
    [SerializeField] private float autoProcessorCost = 10f;
    [SerializeField] private float scalingEngineCost = 50f;
    [SerializeField] private float expansionProtocolCost = 200f;

    [Header("Upgrade Values")]
    [SerializeField] private float autoProcessorBonus = 1f;
    [SerializeField] private float scalingEngineMultiplier = 2f;
    [SerializeField] private float expansionProtocolBonus = 10f;

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
        // Passive output generation
        if (outputsPerSecond > 0)
        {
            float generatedThisFrame = outputsPerSecond * Time.deltaTime;
            outputs += generatedThisFrame;

            // World health degrades based on production rate
            worldHealth -= generatedThisFrame * worldHealthDecayPerOutput;
        }

        // Clamp world health
        worldHealth = Mathf.Max(0, worldHealth);

        // Progress world time
        if (timeProgressionSpeed > 0)
        {
            currentTime += (Time.deltaTime / timeProgressionSpeed); // Convert real time to in-game hours

            // Wrap time at 24 hours (optional - or let it run beyond)
            if (currentTime >= 24f)
            {
                currentTime = currentTime % 24f;
            }
        }

        // Check for Day 5 ending condition
        if (currentDay == 5 && currentTime >= 18f && dailyPoints < dailyQuota)
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.TriggerEnding();
            }
        }

        // Check quota completion
        UpdateSleepButton();

        UpdateUI();
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
    /// Attempt to purchase Auto Processor upgrade
    /// Adds passive outputs per second
    /// </summary>
    public bool PurchaseAutoProcessor()
    {
        if (outputs >= autoProcessorCost)
        {
            outputs -= autoProcessorCost;
            outputsPerSecond += autoProcessorBonus;
            autoProcessorCost *= 1.5f; // Increase cost for next purchase
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempt to purchase Scaling Engine upgrade
    /// Multiplies outputs per click
    /// </summary>
    public bool PurchaseScalingEngine()
    {
        if (outputs >= scalingEngineCost)
        {
            outputs -= scalingEngineCost;
            outputsPerClick *= scalingEngineMultiplier;
            scalingEngineCost *= 2f; // Increase cost for next purchase
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempt to purchase Expansion Protocol upgrade
    /// Significantly increases passive generation
    /// </summary>
    public bool PurchaseExpansionProtocol()
    {
        if (outputs >= expansionProtocolCost)
        {
            outputs -= expansionProtocolCost;
            outputsPerSecond += expansionProtocolBonus;
            expansionProtocolCost *= 2.5f; // Increase cost for next purchase
            return true;
        }
        return false;
    }

    /// <summary>
    /// Update UI displays
    /// </summary>
    private void UpdateUI()
    {
        if (outputsText != null)
        {
            outputsText.text = $"Outputs: {outputs:F0}";
        }

        if (outputsPerSecondText != null)
        {
            outputsPerSecondText.text = $"Per Second: {outputsPerSecond:F1}";
        }

        if (quotaText != null)
        {
            quotaText.text = $"Quota: {dailyPoints:F0}/{dailyQuota:F0}";
        }

        if (dayText != null)
        {
            dayText.text = $"Day {currentDay}";
        }

        if (timeText != null)
        {
            timeText.text = $"Time: {FormatTime(currentTime)}";
        }
    }

    /// <summary>
    /// Add outputs to the player's total (used by ticket system)
    /// </summary>
    public void AddOutputs(float amount)
    {
        outputs += amount;
        dailyPoints += amount; // Track daily progress toward quota
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
    /// Start a new day - called at game start and after sleeping
    /// </summary>
    private void StartNewDay()
    {
        currentTime = startTime; // Reset to 6:00 AM
        dailyPoints = 0f;

        // Day 5: set impossible quota
        if (currentDay == 5)
        {
            dailyQuota = 999999f;
        }
        else
        {
            dailyQuota = baseQuota * Mathf.Pow(quotaMultiplier, currentDay - 1);
        }

        SystemLog.Instance?.LogMessage($"Day {currentDay} begins. Quota: {dailyQuota:F0} points");
    }

    /// <summary>
    /// Called when player clicks sleep button
    /// </summary>
    public void OnSleep()
    {
        if (dailyPoints < dailyQuota)
        {
            SystemLog.Instance?.LogMessage("Quota not met! Keep working.");
            return;
        }

        // Show upgrade modal and let DayManager handle advancement
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnSleepClicked();
        }
    }

    /// <summary>
    /// Advance to next day (called by DayManager after upgrade choice)
    /// </summary>
    public void AdvanceToNextDay()
    {
        currentDay++;
        StartNewDay();
        UpdateUI();
    }

    /// <summary>
    /// Update sleep button enabled state based on quota
    /// </summary>
    private void UpdateSleepButton()
    {
        if (sleepButton != null)
        {
            bool quotaMet = dailyPoints >= dailyQuota;
            sleepButton.interactable = quotaMet;
        }
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
    public float GetOutputs() => outputs;
    public float GetAutoProcessorCost() => autoProcessorCost;
    public float GetScalingEngineCost() => scalingEngineCost;
    public float GetExpansionProtocolCost() => expansionProtocolCost;
    public int GetCurrentDay() => currentDay;
    public float GetDailyQuota() => dailyQuota;
    public float GetDailyPoints() => dailyPoints;
}
