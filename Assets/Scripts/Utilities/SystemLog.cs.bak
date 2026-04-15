using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays system messages that change based on world health.
/// Provides subtle narrative feedback about system degradation.
/// </summary>
public class SystemLog : MonoBehaviour
{
    public static SystemLog Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI logText;

    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 2f; // Update every 2 seconds

    private float updateTimer = 0f;
    private string customMessage = null;
    private float customMessageDuration = 3f;
    private float customMessageTimer = 0f;

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

    private void Update()
    {
        // Handle custom message timer
        if (customMessage != null)
        {
            customMessageTimer += Time.deltaTime;

            if (customMessageTimer >= customMessageDuration)
            {
                customMessage = null;
                customMessageTimer = 0f;
            }
        }

        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateLogMessage();
        }
    }

    /// <summary>
    /// Update the log message based on current world health
    /// </summary>
    private void UpdateLogMessage()
    {
        if (logText == null || GameManager.Instance == null) return;

        // If there's a custom message active, show it instead
        if (customMessage != null)
        {
            logText.text = customMessage;
            return;
        }

        float worldHealth = GameManager.Instance.GetWorldHealth();
        string message = GetMessageForHealth(worldHealth);

        logText.text = message;
    }

    /// <summary>
    /// Select appropriate message based on world health value
    /// </summary>
    private string GetMessageForHealth(float health)
    {
        if (health >= 90f)
        {
            return GetRandomMessage(new string[]
            {
                "SYSTEM STATUS: OPTIMAL",
                "All systems nominal.",
                "Efficiency at maximum.",
                "Processing within normal parameters."
            });
        }
        else if (health >= 70f)
        {
            return GetRandomMessage(new string[]
            {
                "System status: Normal.",
                "Minor fluctuations detected.",
                "All metrics stable.",
                "Performance acceptable."
            });
        }
        else if (health >= 40f)
        {
            return GetRandomMessage(new string[]
            {
                "WARNING: Resource allocation irregular.",
                "External variables showing anomalies.",
                "Non-critical systems experiencing strain.",
                "Efficiency impact detected."
            });
        }
        else if (health >= 15f)
        {
            return GetRandomMessage(new string[]
            {
                "ALERT: Cascading resource failures.",
                "Critical thresholds approaching.",
                "External environment degrading.",
                "System integrity compromised."
            });
        }
        else
        {
            return GetRandomMessage(new string[]
            {
                "CRITICAL: Catastrophic resource depletion.",
                "External collapse imminent.",
                "Irreversible damage detected.",
                "Point of no return exceeded."
            });
        }
    }

    /// <summary>
    /// Returns a random message from the provided array
    /// </summary>
    private string GetRandomMessage(string[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            return "SYSTEM LOG";
        }

        int index = Random.Range(0, messages.Length);
        return messages[index];
    }

    /// <summary>
    /// Display a custom message for a limited time (called by other systems)
    /// </summary>
    public void LogMessage(string message, float duration = 3f)
    {
        customMessage = message;
        customMessageDuration = duration;
        customMessageTimer = 0f;

        // Update immediately to show the message
        if (logText != null)
        {
            logText.text = message;
        }
    }
}
