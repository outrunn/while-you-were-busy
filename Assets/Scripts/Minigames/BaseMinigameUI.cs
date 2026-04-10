using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract base class for all minigames.
/// Provides common lifecycle management and shared behavior.
/// Subclasses implement the game-specific logic in abstract methods.
/// </summary>
public abstract class BaseMinigameUI : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected GameObject minigameWindow;
    [SerializeField] protected float completionDelay = 2f;
    [SerializeField] protected float wrongAnswerFlashDuration = 0.3f;

    // State
    protected bool isActive = false;
    protected bool isCompleted = false;
    protected System.Action onMinigameCompleted;

    /// <summary>
    /// Start the minigame with a completion callback.
    /// Subclasses implement game-specific setup here.
    /// </summary>
    public abstract void StartMinigame(System.Action completionCallback);

    /// <summary>
    /// Close the minigame UI and reset state.
    /// </summary>
    public virtual void CloseMinigame()
    {
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }

        isActive = false;
        isCompleted = false;
    }

    /// <summary>
    /// Mark the minigame as complete and invoke the callback.
    /// </summary>
    protected virtual void CompleteMinigame()
    {
        if (isCompleted) return;

        isCompleted = true;
        isActive = false;

        // Invoke the completion callback
        onMinigameCompleted?.Invoke();

        // Close after a delay so player sees the completion state
        StartCoroutine(CloseAfterDelay(completionDelay));
    }

    /// <summary>
    /// Close the minigame after a specified delay.
    /// </summary>
    protected IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseMinigame();
    }

    /// <summary>
    /// Flash the window background red for wrong answer feedback.
    /// </summary>
    protected IEnumerator FlashRed(Image windowBackground)
    {
        if (windowBackground == null) yield break;

        Color originalColor = windowBackground.color;
        windowBackground.color = Color.red;

        yield return new WaitForSeconds(wrongAnswerFlashDuration);

        windowBackground.color = originalColor;
    }

    /// <summary>
    /// Show the minigame window.
    /// </summary>
    protected void ShowWindow()
    {
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the minigame window.
    /// </summary>
    protected void HideWindow()
    {
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    /// <summary>
    /// Check if the minigame is currently active.
    /// </summary>
    public bool IsActive() => isActive;

    /// <summary>
    /// Check if the minigame is completed.
    /// </summary>
    public bool IsCompleted() => isCompleted;
}
