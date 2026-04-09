using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float expandDuration = 0.3f;

    private GameObject miniView; // Small version on board
    private GameObject expandedView; // Full details view
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

        // Find ExpandedView and make it always active (scale handles mini/expanded visual)
        // No MiniView needed — using scale-only approach for mini/expanded states
        foreach (Transform child in transform)
        {
            if (child.name == "ExpandedView")
            {
                expandedView = child.gameObject;
                expandedView.SetActive(true); // Always visible — scale controls mini vs expanded look
            }
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

        // Dispatch to appropriate minigame based on task type
        switch (taskData.minigameType)
        {
            case MinigameType.Typing:
                if (taskData.typingTask != null && TypingMinigameUI.Instance != null)
                {
                    TypingMinigameUI.Instance.StartMinigame(taskData.typingTask, OnMinigameCompleted);
                    Debug.Log($"Starting typing minigame for: {taskTitle}");
                }
                else
                {
                    Debug.LogError("Typing task data missing or TypingMinigameUI not in scene!");
                }
                break;

            case MinigameType.MultipleChoice:
                Debug.Log($"MC task detected: {taskTitle}. Instance={MultipleChoiceMinigameUI.Instance}");
                if (MultipleChoiceMinigameUI.Instance != null)
                {
                    MultipleChoiceMinigameUI.Instance.StartMinigame(OnMinigameCompleted);
                    Debug.Log($"Starting multiple choice minigame for: {taskTitle}");
                }
                else
                {
                    Debug.LogError("MultipleChoiceMinigameUI not in scene!");
                }
                break;

            case MinigameType.Math:
                if (MathMinigameUI.Instance != null)
                {
                    MathMinigameUI.Instance.StartMinigame(OnMinigameCompleted);
                    Debug.Log($"Starting math minigame for: {taskTitle}");
                }
                else
                {
                    Debug.LogError("MathMinigameUI not in scene!");
                }
                break;

            case MinigameType.PhotoReveal:
                if (PhotoRevealMinigameUI.Instance != null)
                {
                    PhotoRevealMinigameUI.Instance.StartMinigame(OnMinigameCompleted);
                    Debug.Log($"Starting photo reveal minigame for: {taskTitle}");
                }
                else
                {
                    Debug.LogError("PhotoRevealMinigameUI not in scene!");
                }
                break;

            default:
                Debug.LogError($"Unknown minigame type: {taskData.minigameType}");
                break;
        }
    }

    /// <summary>
    /// Called when the minigame is completed
    /// Stamps the ticket - player must then drag to processing machine for rewards
    /// </summary>
    private void OnMinigameCompleted()
    {
        Debug.Log($"Minigame completed for task: {taskTitle}");

        // Increment the day's task completion counter
        GameManager.Instance?.CompleteTask();

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

            // Ensure stamped ticket is expanded (so player can drag it to shredder)
            if (!isExpanded)
            {
                isExpanded = true;
                if (expandCoroutine != null)
                {
                    StopCoroutine(expandCoroutine);
                }
                // Move to root canvas so expanded stamped ticket renders above all UI
                transform.SetParent(canvas.transform);
                transform.SetAsLastSibling();
                rectTransform.localScale = Vector3.one;
            }

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
        Debug.Log($"Ticket.OnBeginDrag: {taskTitle}");
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        // Re-parent to root canvas for proper z-ordering during drag (renders above all UI)
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
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
        Debug.Log($"===== Ticket.OnEndDrag: {taskTitle} =====");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Raycast to find what's under the mouse pointer at drop position
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log($"Raycast hit: {result.gameObject.name}");
                ShredderUI shredder = result.gameObject.GetComponent<ShredderUI>();
                if (shredder != null)
                {
                    Debug.Log($"===== FOUND SHREDDER! Calling OnDrop =====");
                    shredder.OnDrop(eventData);
                    Debug.Log($"===== OnDrop call complete =====");
                    return;
                }
            }
        }

        Debug.Log("No shredder found at drop location");

        // No drop target found — keep ticket at new position but restore parent
        // (don't reset position - let the player place tickets where they dragged them)
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }
    }

    /// <summary>
    /// Handle click on ticket to toggle expanded view
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Don't allow toggling stamped tickets - they should stay expanded for dragging to shredder
        if (!isStamped)
        {
            ToggleExpand();
        }
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

        // Save current parent before expanding (so we can return on collapse)
        if (!isExpanded)
            originalParent = transform.parent;

        expandCoroutine = StartCoroutine(ExpandCoroutine(!isExpanded));
    }

    /// <summary>
    /// Coroutine to animate expand/collapse
    /// </summary>
    private IEnumerator ExpandCoroutine(bool shouldExpand)
    {
        // Re-parent to root canvas when expanding so ticket renders above all other UI
        if (shouldExpand && canvas != null)
        {
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
        }

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

        // Keep ticket at root canvas regardless of expand/collapse state
        // (so it always stays visible above background elements like the BulletinBoard)
        if (canvas != null && transform.parent != canvas.transform)
        {
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
        }

        expandCoroutine = null;
    }

    // Public getters
    public bool IsStamped() => isStamped;
    public bool IsPinned() => isPinned;
    public float GetReward() => rewardAmount;
    public string GetTaskTitle() => taskTitle;
    public string GetTaskDescription() => taskDescription;

    /// <summary>
    /// Reset ticket position if drag was invalid
    /// </summary>
    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    /// <summary>
    /// Reset ticket state for object pool reuse
    /// </summary>
    public void ResetForReuse()
    {
        isStamped = false;
        isPinned = false;
        isExpanded = false;
        taskTitle = "";
        taskDescription = "";
        rewardAmount = 0f;
        taskData = null;

        if (stampImage != null)
        {
            stampImage.gameObject.SetActive(false);
        }

        if (startTaskButton != null)
        {
            startTaskButton.gameObject.SetActive(false);
        }

        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
            expandCoroutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        UpdateUI();
    }
}
