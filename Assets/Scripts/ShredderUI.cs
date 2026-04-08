using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Drop zone for the shredder - allows discarding unwanted tickets.
/// Acts as an alternative to the bulletin board for failed or unwanted tickets.
/// </summary>
public class ShredderUI : MonoBehaviour, IDropHandler
{
    [Header("Shredder Settings")]
    [SerializeField] private bool allowDrops = true;

    [Header("Visual Feedback")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    private Image shredderImage;

    private void Start()
    {
        shredderImage = GetComponent<Image>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Add raycast target if not already
        if (shredderImage != null)
        {
            shredderImage.raycastTarget = true;
        }
        else
        {
            Debug.LogWarning("ShredderUI: No Image component found! Adding one...");
            shredderImage = gameObject.AddComponent<Image>();
            shredderImage.raycastTarget = true;
        }

        // Ensure GraphicRaycaster exists for drop detection
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogWarning("ShredderUI: No GraphicRaycaster found! Adding one...");
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Add event trigger for hover effects
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((data) => OnPointerEnter((PointerEventData)data));
        trigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((data) => OnPointerExit((PointerEventData)data));
        trigger.triggers.Add(pointerExitEntry);
    }

    /// <summary>
    /// Handle ticket being dropped on shredder
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("ShredderUI.OnDrop called!");

        if (!allowDrops)
        {
            Debug.Log("ShredderUI: Drops not allowed");
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;
        Debug.Log($"Dropped object: {(droppedObject != null ? droppedObject.name : "NULL")}");

        if (droppedObject != null)
        {
            Debug.Log($"Dropped object: {droppedObject.name}");
            Ticket ticket = droppedObject.GetComponent<Ticket>();

            if (ticket != null)
            {
                Debug.Log($"Ticket found: {ticket.GetTaskTitle()}, IsStamped: {ticket.IsStamped()}");
                // Only shred completed (stamped) tickets
                if (ticket.IsStamped())
                {
                    Debug.Log("Ticket is stamped - shredding!");
                    ShredTicket(ticket);
                }
                else
                {
                    Debug.Log("Ticket is NOT stamped - resetting position");
                    // Reset position if not stamped
                    ticket.ResetPosition();
                    SystemLog.Instance?.LogMessage("Can only shred completed tickets!");
                }
            }
            else
            {
                Debug.Log($"No Ticket component found on {droppedObject.name}");
            }
        }
        else
        {
            Debug.Log("No object was dragged to shredder");
        }
    }

    /// <summary>
    /// Destroy a completed ticket in the shredder and award quota
    /// </summary>
    private void ShredTicket(Ticket ticket)
    {
        string taskTitle = ticket.GetTaskTitle();
        float reward = ticket.GetReward();

        // Award quota points for the completed ticket
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.AddOutputs(reward);
            gameManager.CompleteTask();
            SystemLog.Instance?.LogMessage($"Shredded '{taskTitle}' - Awarded {reward} quota!");
        }
        else
        {
            SystemLog.Instance?.LogMessage($"Shredded: {taskTitle}");
        }

        // Play destruction animation or sound here
        PlayShredAnimation();

        // Notify printer that ticket is being processed
        Printer printer = FindFirstObjectByType<Printer>();
        if (printer != null)
        {
            printer.OnTicketProcessed();
        }

        // Destroy the ticket
        Destroy(ticket.gameObject);
    }

    private void PlayShredAnimation()
    {
        // Could add particle effects, rotation animation, or sound here
        // For now, just a simple feedback
        Debug.Log("Shredding ticket...");
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = activeAlpha;
        }
    }

    private void OnPointerExit(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = inactiveAlpha;
        }
    }

    public void SetAllowDrops(bool allow)
    {
        allowDrops = allow;
    }

    public bool IsAcceptingDrops() => allowDrops;
}
