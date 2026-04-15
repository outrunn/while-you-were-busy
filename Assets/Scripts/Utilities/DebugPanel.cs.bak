using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Debug panel for quick testing and validation.
/// Shows current game state and provides buttons to test key mechanics.
/// Only available in Editor builds.
/// </summary>
public class DebugPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private void Start()
    {
        if (panelRoot == null)
        {
            Debug.LogWarning("DebugPanel: panelRoot not assigned. Creating basic panel...");
            CreateDebugPanel();
        }

        CreateDebugButtons();
        UpdateStateDisplay();
    }

    private void Update()
    {
        // Toggle panel with ~ key
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(!panelRoot.activeSelf);
            }
        }

        UpdateStateDisplay();
    }

    /// <summary>
    /// Create basic UI panel if not assigned
    /// </summary>
    private void CreateDebugPanel()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("DebugPanel: No Canvas found in scene!");
            return;
        }

        // Create panel
        GameObject panelObj = new GameObject("DebugPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.zero;
        panelRect.offsetMin = new Vector2(10, 10);
        panelRect.offsetMax = new Vector2(310, 500);

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // Create title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(5, -25);
        titleRect.offsetMax = new Vector2(-5, -5);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "DEBUG PANEL (~)";
        titleText.fontSize = 20;

        // Create state text
        GameObject stateObj = new GameObject("StateText");
        stateObj.transform.SetParent(panelObj.transform, false);
        RectTransform stateRect = stateObj.AddComponent<RectTransform>();
        stateRect.anchorMin = new Vector2(0, 1);
        stateRect.anchorMax = new Vector2(1, 1);
        stateRect.offsetMin = new Vector2(5, -150);
        stateRect.offsetMax = new Vector2(-5, -30);

        stateText = stateObj.AddComponent<TextMeshProUGUI>();
        stateText.fontSize = 14;

        // Create button container
        GameObject containerObj = new GameObject("ButtonContainer");
        containerObj.transform.SetParent(panelObj.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.offsetMin = new Vector2(5, 5);
        containerRect.offsetMax = new Vector2(-5, -155);

        VerticalLayoutGroup layout = containerObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        buttonContainer = containerObj.transform;
        panelRoot = panelObj;
    }

    /// <summary>
    /// Create all debug buttons
    /// </summary>
    private void CreateDebugButtons()
    {
        AddButton("Add 50 Outputs", () => AddOutputs(50));
        AddButton("Add 100 Outputs", () => AddOutputs(100));
        AddButton("Complete Quota", CompleteQuota);
        AddButton("Next Day", AdvanceDay);
        AddButton("Day 5", () => SetDay(5));
        AddButton("Print Ticket", PrintTicket);
        AddButton("Start Typing Game", StartTypingGame);
        AddButton("Damage Health -50", () => DamageHealth(50));
        AddButton("Full Health", FullHealth);
        AddButton("Trigger Ending", TriggerEnding);
    }

    /// <summary>
    /// Add a debug button
    /// </summary>
    private void AddButton(string label, System.Action callback)
    {
        if (buttonContainer == null) return;

        GameObject buttonObj = new GameObject(label);
        buttonObj.transform.SetParent(buttonContainer, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 30);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        TextMeshProUGUI buttonText = buttonObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = label;
        buttonText.fontSize = 12;
        buttonText.alignment = TextAlignmentOptions.Center;

        button.onClick.AddListener(() => callback?.Invoke());
    }

    /// <summary>
    /// Update the state display text
    /// </summary>
    private void UpdateStateDisplay()
    {
        if (stateText == null || GameManager.Instance == null) return;

        stateText.text = $"Day: {GameManager.Instance.GetCurrentDay()}\n" +
                         $"Quota: {GameManager.Instance.GetDailyPoints():F0}/{GameManager.Instance.GetDailyQuota():F0}\n" +
                         $"Outputs: {GameManager.Instance.GetOutputs():F0}\n" +
                         $"Health: {GameManager.Instance.GetWorldHealth():F0}%\n" +
                         $"Click Power: x1";
    }

    // === Debug Actions ===

    private void AddOutputs(float amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddOutputs(amount);
            SystemLog.Instance?.LogMessage($"[DEBUG] Added {amount} outputs");
        }
    }

    private void CompleteQuota()
    {
        if (GameManager.Instance == null) return;

        float needed = GameManager.Instance.GetDailyQuota() - GameManager.Instance.GetDailyPoints();
        GameManager.Instance.AddOutputs(needed);
        SystemLog.Instance?.LogMessage("[DEBUG] Quota completed");
    }

    private void AdvanceDay()
    {
        if (GameManager.Instance == null) return;

        int nextDay = GameManager.Instance.GetCurrentDay() + 1;
        if (nextDay > 5) nextDay = 5;
        SetDay(nextDay);
    }

    private void SetDay(int day)
    {
        if (GameManager.Instance == null) return;

        while (GameManager.Instance.GetCurrentDay() < day)
        {
            GameManager.Instance.AdvanceToNextDay();
        }

        SystemLog.Instance?.LogMessage($"[DEBUG] Set to Day {day}");
    }

    private void PrintTicket()
    {
        Printer printer = FindFirstObjectByType<Printer>();
        if (printer != null)
        {
            printer.PrintTicket();
            SystemLog.Instance?.LogMessage("[DEBUG] Ticket printed");
        }
        else
        {
            Debug.LogWarning("Printer not found!");
        }
    }

    private void StartTypingGame()
    {
        // Find first unstamped ticket and start its typing game
        var tickets = FindObjectsByType<Ticket>(FindObjectsSortMode.None);
        foreach (var ticket in tickets)
        {
            if (!ticket.IsStamped())
            {
                ticket.GetComponent<Button>()?.onClick.Invoke();
                SystemLog.Instance?.LogMessage("[DEBUG] Started typing game on first available ticket");
                return;
            }
        }

        SystemLog.Instance?.LogMessage("[DEBUG] No unstamped tickets found");
    }

    private void DamageHealth(float amount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DegradeWorldHealth(amount);
            SystemLog.Instance?.LogMessage($"[DEBUG] Health damaged by {amount}");
        }
    }

    private void FullHealth()
    {
        if (GameManager.Instance != null)
        {
            // Can't restore health directly, but this shows the concept
            SystemLog.Instance?.LogMessage("[DEBUG] Health restore not implemented");
        }
    }

    private void TriggerEnding()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.TriggerEnding();
            SystemLog.Instance?.LogMessage("[DEBUG] Day 5 ending triggered");
        }
    }
}
#endif
