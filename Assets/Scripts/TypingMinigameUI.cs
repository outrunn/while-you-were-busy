using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the typing minigame UI window.
/// Displays a fake Google Docs interface where the player spams keys to "type" a predetermined message.
/// </summary>
public class TypingMinigameUI : MonoBehaviour
{
    public static TypingMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameWindow;
    [SerializeField] private TextMeshProUGUI documentTitleText;
    [SerializeField] private TextMeshProUGUI documentContentText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progressBar;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f; // TODO: implement variable typing speed
    [SerializeField] private bool showProgressBar = true;

    // Current minigame state
    private TypingTaskSO currentTask;
    private string fullMessage;
    private int currentCharacterIndex = 0;
    private int requiredKeyPresses = 0;
    private int currentKeyPresses = 0;
    private bool isActive = false;
    private bool isCompleted = false;

    // Completion callback
    private System.Action onMinigameCompleted;

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
            CompleteMinigame();
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
    /// Marks the minigame as complete and triggers callback
    /// </summary>
    private void CompleteMinigame()
    {
        if (isCompleted) return;

        isCompleted = true;
        isActive = false;

        // Show full message
        if (documentContentText != null && currentCharacterIndex < fullMessage.Length)
        {
            documentContentText.text = fullMessage;
        }

        Debug.Log("Typing minigame completed!");

        // Auto-close after delay
        float delay = currentTask != null ? currentTask.completionDelay : 2.5f;
        StartCoroutine(CloseAfterDelay(delay));
    }

    /// <summary>
    /// Closes the minigame window after a delay and invokes completion callback
    /// </summary>
    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Invoke completion callback before closing
        onMinigameCompleted?.Invoke();

        CloseMinigame();
    }

    /// <summary>
    /// Closes the minigame window immediately
    /// </summary>
    public void CloseMinigame()
    {
        isActive = false;
        isCompleted = false;
        currentTask = null;
        onMinigameCompleted = null;

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
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

    #region Helper Methods

    /// <summary>
    /// Gets the current completion percentage
    /// </summary>
    public float GetCompletionPercentage()
    {
        if (requiredKeyPresses <= 0) return 0f;
        return Mathf.Clamp01((float)currentKeyPresses / requiredKeyPresses);
    }

    /// <summary>
    /// Checks if the minigame is currently active
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }

    /// <summary>
    /// Checks if the minigame is completed
    /// </summary>
    public bool IsCompleted()
    {
        return isCompleted;
    }

    #endregion
}
