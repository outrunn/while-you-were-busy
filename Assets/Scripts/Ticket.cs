using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Represents a task ticket that can be printed, pinned, stamped, and processed.
/// Supports mini form (pinned on board) and expanded form (shows details).
/// </summary>
public class Ticket : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Ticket Data")]
    [SerializeField] private string taskTitle;
    [SerializeField] private string taskDescription;
    [SerializeField] private float rewardAmount = 10f;
    [SerializeField] private int difficultyLevel = 1;
    [SerializeField] private TaskData taskData; // Store full task data for minigame access

    [Header("Ticket State")]
    [SerializeField] private bool isStamped = false;
    [SerializeField] private bool isPinned = false;
    private bool isExpanded = false;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image stampImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button startTaskButton; // Button to start minigame

    [Header("Mini/Expand Views")]
    [SerializeField] private GameObject miniView; // Small version on board
    [SerializeField] private GameObject expandedView; // Full details view
    [SerializeField] private float expandDuration = 0.3f;

    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Coroutine expandCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Find the root canvas (handles nested canvas cases)
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null) canvas = canvas.rootCanvas;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (stampImage != null)
        {
            stampImage.gameObject.SetActive(false);
        }

        // Setup start task button
        if (startTaskButton != null)
        {
            startTaskButton.onClick.AddListener(OnStartTaskButtonClicked);
            startTaskButton.gameObject.SetActive(false); // Hidden by default
        }

        // Setup mini/expanded views
        if (miniView != null)
        {
            miniView.SetActive(true);
        }

        if (expandedView != null)
        {
            expandedView.SetActive(false);
        }
    }

    /// <summary>
    /// Initialize ticket with task data
    /// </summary>
    public void Initialize(string title, string description, float reward, int difficulty)
    {
        taskTitle = title;
        taskDescription = description;
        rewardAmount = reward;
        difficultyLevel = difficulty;

        UpdateUI();
    }

    /// <summary>
    /// Initialize ticket with full TaskData (for minigame support)
    /// </summary>
    public void Initialize(TaskData data)
    {
        taskData = data;
        taskTitle = data.title;
        taskDescription = data.description;
        rewardAmount = data.baseReward;
        difficultyLevel = data.difficultyLevel;

        UpdateUI();
        UpdateMinigameButton();
    }

    /// <summary>
    /// Update ticket UI display
    /// </summary>
    private void UpdateUI()
    {
        if (titleText != null)
        {
            titleText.text = taskTitle;
        }

        if (descriptionText != null)
        {
            descriptionText.text = taskDescription;
        }
    }

    /// <summary>
    /// Show/hide the start task button based on whether this task requires a minigame
    /// </summary>
    private void UpdateMinigameButton()
    {
        if (startTaskButton == null) return;

        bool shouldShowButton = taskData != null && taskData.requiresMinigame;
        startTaskButton.gameObject.SetActive(shouldShowButton);

        // Make button more prominent if task is not yet completed
        if (shouldShowButton && !isStamped)
        {
            // You can customize button appearance here in Inspector or via code
            // Example: startTaskButton.GetComponent<Image>().color = Color.green;
        }
    }

    /// <summary>
    /// Called when the Start Task button is clicked
    /// </summary>
    private void OnStartTaskButtonClicked()
    {
        if (taskData == null || !taskData.requiresMinigame)
        {
            Debug.LogWarning("This task does not require a minigame!");
            return;
        }

        if (isStamped)
        {
            Debug.Log("Task already completed!");
            return;
        }

        // Start typing minigame
        if (taskData.typingTask != null && TypingMinigameUI.Instance != null)
        {
            TypingMinigameUI.Instance.StartMinigame(taskData.typingTask, OnMinigameCompleted);
            Debug.Log($"Starting typing minigame for: {taskTitle}");
        }
        else
        {
            Debug.LogError("Typing task data is missing or TypingMinigameUI is not in scene!");
        }
    }

    /// <summary>
    /// Called when the minigame is completed
    /// Stamps the ticket - player must then drag to processing machine for rewards
    /// </summary>
    private void OnMinigameCompleted()
    {
        Debug.Log($"Minigame completed for task: {taskTitle}");

        // Only stamp the ticket - rewards/health degradation happen in ProcessingMachine
        StampTicket();

        // Log completion
        SystemLog.Instance?.LogMessage($"Task completed! Drag to processing machine.");
    }

    /// <summary>
    /// Mark ticket as stamped/completed
    /// </summary>
    public void StampTicket()
    {
        if (!isStamped)
        {
            isStamped = true;

            // Show approval stamp animation over this ticket
            TicketCompletionStamp stampAnimator = FindFirstObjectByType<TicketCompletionStamp>();
            if (stampAnimator != null)
            {
                stampAnimator.ShowStampAtTicket(GetComponent<RectTransform>());
            }
            else
            {
                // Fallback: show the stamp image if no animator found
                if (stampImage != null)
                {
                    stampImage.gameObject.SetActive(true);
                }
            }

            // Hide the START TASK button after completion
            if (startTaskButton != null)
            {
                startTaskButton.gameObject.SetActive(false);
            }

            SystemLog.Instance?.LogMessage($"Task stamped: {taskTitle}");
        }
    }

    /// <summary>
    /// Pin ticket to bulletin board
    /// </summary>
    public void Pin(Transform parent)
    {
        isPinned = true;
        transform.SetParent(parent);
        originalParent = parent;
    }

    /// <summary>
    /// Unpin ticket from bulletin board
    /// </summary>
    public void Unpin()
    {
        isPinned = false;
    }

    // Drag handlers for moving tickets around
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint
            );
            rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if dropped on valid target (bulletin board or processing machine)
        // This will be handled by drop zones
    }

    /// <summary>
    /// Handle click on ticket to toggle expanded view
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleExpand();
    }

    /// <summary>
    /// Toggle between mini and expanded views with scale animation
    /// </summary>
    public void ToggleExpand()
    {
        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
        }

        expandCoroutine = StartCoroutine(ExpandCoroutine(!isExpanded));
    }

    /// <summary>
    /// Coroutine to animate expand/collapse
    /// </summary>
    private IEnumerator ExpandCoroutine(bool shouldExpand)
    {
        Vector3 startScale = rectTransform.localScale;
        Vector3 endScale = shouldExpand ? Vector3.one : new Vector3(0.3f, 0.3f, 1f);

        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / expandDuration;
            t = Mathf.SmoothStep(0, 1, t);

            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        rectTransform.localScale = endScale;
        isExpanded = shouldExpand;

        // Swap views
        if (miniView != null)
        {
            miniView.SetActive(!shouldExpand);
        }

        if (expandedView != null)
        {
            expandedView.SetActive(shouldExpand);
        }

        expandCoroutine = null;
    }

    // Public getters
    public bool IsStamped() => isStamped;
    public bool IsPinned() => isPinned;
    public float GetReward() => rewardAmount;
    public string GetTaskTitle() => taskTitle;
    public string GetTaskDescription() => taskDescription;
    public int GetDifficulty() => difficultyLevel;

    /// <summary>
    /// Reset ticket position if drag was invalid
    /// </summary>
    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}
