using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Typing minigame: player spams keys to "type" a predetermined message.
/// Displays a fake Google Docs interface with text reveal animation.
/// </summary>
public class TypingMinigameUI : BaseMinigameUI
{
    public static TypingMinigameUI Instance { get; private set; }

    [Header("Typing UI References")]
    [SerializeField] private TextMeshProUGUI documentTitleText;
    [SerializeField] private TextMeshProUGUI documentContentText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progressBar;

    [Header("Typing Settings")]
    [SerializeField] private bool showProgressBar = true;

    // Game-specific state
    private TypingTaskSO currentTask;
    private string fullMessage;
    private int currentCharacterIndex = 0;
    private int requiredKeyPresses = 0;
    private int currentKeyPresses = 0;

    // Canvas reference for layering
    private Canvas minigameCanvas;

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

        // Setup Canvas for top-layer rendering
        minigameCanvas = GetComponent<Canvas>();
        if (minigameCanvas != null)
        {
            minigameCanvas.sortingOrder = 100;
            minigameCanvas.overrideSorting = true;
        }
        else
        {
            Debug.LogWarning("TypingMinigameUI: Canvas component not found! Minigame may not render on top.");
        }

        // Hide window on start
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
        else
        {
            Debug.LogWarning("TypingMinigameUI: 'Minigame Window' is not assigned in the Inspector. " +
                             "The minigame UI will always be visible. Please assign the panel GameObject to fix this.");
        }
    }

    private void Update()
    {
        if (!isActive || isCompleted) return;

        // Listen for any key press (except escape to allow closing)
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            OnKeyPressed();
        }
    }

    /// <summary>
    /// Stores task for later use (called before StartMinigame)
    /// </summary>
    public void SetTask(TypingTaskSO task)
    {
        currentTask = task;
    }

    /// <summary>
    /// Abstract method implementation — this version accepts a TypingTaskSO
    /// </summary>
    public override void StartMinigame(System.Action completionCallback)
    {
        if (currentTask != null)
        {
            StartMinigame(currentTask, completionCallback);
        }
        else
        {
            Debug.LogWarning("TypingMinigameUI.StartMinigame(Action) called without task data. Call SetTask first.");
            onMinigameCompleted = completionCallback;
        }
    }

    /// <summary>
    /// Opens the typing minigame with the specified task
    /// </summary>
    public void StartMinigame(TypingTaskSO task, System.Action completionCallback)
    {
        if (task == null)
        {
            Debug.LogError("Cannot start typing minigame: task is null!");
            return;
        }

        currentTask = task;
        fullMessage = task.messageToType;
        requiredKeyPresses = task.GetRequiredKeyPresses();
        onMinigameCompleted = completionCallback;

        // Reset state
        currentCharacterIndex = 0;
        currentKeyPresses = 0;
        isActive = true;
        isCompleted = false;

        // Setup UI
        if (documentTitleText != null)
        {
            documentTitleText.text = task.taskTitle;
        }

        if (documentContentText != null)
        {
            documentContentText.text = ""; // Start empty
        }

        UpdateProgress();

        // Show window
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }

        Debug.Log($"Typing minigame started: {task.taskTitle}");
    }

    /// <summary>
    /// Called when the player presses any key
    /// </summary>
    private void OnKeyPressed()
    {
        if (!isActive || isCompleted) return;

        currentKeyPresses++;

        // Reveal next character(s) based on typing speed
        if (currentCharacterIndex < fullMessage.Length)
        {
            currentCharacterIndex++;

            if (documentContentText != null)
            {
                documentContentText.text = fullMessage.Substring(0, currentCharacterIndex);
            }
        }

        UpdateProgress();

        // Check if completed
        if (currentKeyPresses >= requiredKeyPresses || currentCharacterIndex >= fullMessage.Length)
        {
            CompleteMinigameTyping();
        }
    }

    /// <summary>
    /// Updates the progress display
    /// </summary>
    private void UpdateProgress()
    {
        float progress = (float)currentKeyPresses / requiredKeyPresses;

        if (progressBar != null && showProgressBar)
        {
            progressBar.fillAmount = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{currentKeyPresses} / {requiredKeyPresses}";
        }
    }

    /// <summary>
    /// Complete the typing minigame and show the full message
    /// </summary>
    private void CompleteMinigameTyping()
    {
        if (isCompleted) return;

        // Show full message
        if (documentContentText != null && currentCharacterIndex < fullMessage.Length)
        {
            documentContentText.text = fullMessage;
        }

        Debug.Log("Typing minigame completed!");

        // Use the base class CompleteMinigame with custom delay
        float delay = currentTask != null ? currentTask.completionDelay : 2.5f;
        isCompleted = true;
        isActive = false;
        onMinigameCompleted?.Invoke();
        StartCoroutine(CloseAfterDelay(delay));
    }

    /// <summary>
    /// Override CloseMinigame to reset game state
    /// </summary>
    public override void CloseMinigame()
    {
        base.CloseMinigame();
        currentTask = null;
    }

    /// <summary>
    /// Manual close (for X button on UI if you add one)
    /// </summary>
    public void OnCloseButtonClicked()
    {
        // Only allow manual close if completed
        if (isCompleted)
        {
            CloseMinigame();
        }
        else
        {
            Debug.Log("Complete the typing task first!");
        }
    }

    /// <summary>
    /// Gets the current completion percentage
    /// </summary>
    public float GetCompletionPercentage()
    {
        if (requiredKeyPresses <= 0) return 0f;
        return Mathf.Clamp01((float)currentKeyPresses / requiredKeyPresses);
    }
}
