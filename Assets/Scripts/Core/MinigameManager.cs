using UnityEngine;

/// <summary>
/// Centralized manager for all minigames.
/// Ensures only one minigame is open at a time.
/// Prevents minigames from opening on top of each other.
/// </summary>
public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("Minigame References")]
    [SerializeField] private TypingMinigameUI typingMinigame;
    [SerializeField] private MathMinigameUI mathMinigame;
    [SerializeField] private MultipleChoiceMinigameUI multipleChoiceMinigame;
    [SerializeField] private PhotoRevealMinigameUI photoRevealMinigame;

    private BaseMinigameUI currentlyOpenMinigame = null;

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
        // Auto-discover minigames if not assigned
        if (typingMinigame == null)
            typingMinigame = FindFirstObjectByType<TypingMinigameUI>();
        if (mathMinigame == null)
            mathMinigame = FindFirstObjectByType<MathMinigameUI>();
        if (multipleChoiceMinigame == null)
            multipleChoiceMinigame = FindFirstObjectByType<MultipleChoiceMinigameUI>();
        if (photoRevealMinigame == null)
            photoRevealMinigame = FindFirstObjectByType<PhotoRevealMinigameUI>();
    }

    /// <summary>
    /// Open a minigame by type. Closes any currently open minigame first.
    /// </summary>
    public void OpenMinigame(MinigameType type, System.Action onComplete)
    {
        // Close any currently open minigame
        if (currentlyOpenMinigame != null)
        {
            currentlyOpenMinigame.CloseMinigame();
        }

        // Get the minigame for this type
        BaseMinigameUI minigame = GetMinigameByType(type);

        if (minigame != null)
        {
            currentlyOpenMinigame = minigame;

            // Open the minigame with the completion callback
            switch (type)
            {
                case MinigameType.Typing:
                    if (typingMinigame != null)
                    {
                        // TypingMinigameUI.StartMinigame requires a TypingTaskSO parameter
                        // This is handled by Ticket directly; MinigameManager just tracks it
                        currentlyOpenMinigame = typingMinigame as BaseMinigameUI;
                    }
                    break;

                case MinigameType.Math:
                    if (mathMinigame != null)
                    {
                        mathMinigame.StartMinigame(onComplete);
                    }
                    break;

                case MinigameType.MultipleChoice:
                    if (multipleChoiceMinigame != null)
                    {
                        multipleChoiceMinigame.StartMinigame(onComplete);
                    }
                    break;

                case MinigameType.PhotoReveal:
                    if (photoRevealMinigame != null)
                    {
                        photoRevealMinigame.StartMinigame(onComplete);
                    }
                    break;
            }
        }
        else
        {
            SystemLog.Instance?.LogMessage($"Minigame {type} not found in scene!");
        }
    }

    /// <summary>
    /// Close the currently open minigame.
    /// </summary>
    public void CloseCurrentMinigame()
    {
        if (currentlyOpenMinigame != null)
        {
            currentlyOpenMinigame.CloseMinigame();
            currentlyOpenMinigame = null;
        }
    }

    /// <summary>
    /// Get the minigame instance for a given type.
    /// </summary>
    private BaseMinigameUI GetMinigameByType(MinigameType type)
    {
        return type switch
        {
            MinigameType.Typing => typingMinigame,
            MinigameType.Math => mathMinigame,
            MinigameType.MultipleChoice => multipleChoiceMinigame,
            MinigameType.PhotoReveal => photoRevealMinigame,
            _ => null
        };
    }

    /// <summary>
    /// Check if a minigame is currently open.
    /// </summary>
    public bool IsMinigameOpen() => currentlyOpenMinigame != null;

    /// <summary>
    /// Get the currently open minigame (if any).
    /// </summary>
    public BaseMinigameUI GetCurrentMinigame() => currentlyOpenMinigame;
}
