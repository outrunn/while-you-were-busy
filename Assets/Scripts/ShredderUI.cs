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
        if (!allowDrops) return;

        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Ticket ticket = droppedObject.GetComponent<Ticket>();

            if (ticket != null)
            {
                // Only shred completed (stamped) tickets
                if (ticket.IsStamped())
                {
                    ShredTicket(ticket);
                }
                else
                {
                    // Reset position if not stamped
                    ticket.ResetPosition();
                    SystemLog.Instance?.LogMessage("Can only shred completed tickets!");
                }
            }
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
