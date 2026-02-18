using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Stamp tool for marking tickets as completed.
/// Player drags tickets onto the stamp or clicks stamp then clicks ticket.
/// </summary>
public class Stamp : MonoBehaviour, IDropHandler
{
    [Header("Stamp Settings")]
    [SerializeField] private bool clickToStampMode = true; // If true, click stamp then click ticket

    [Header("UI References")]
    [SerializeField] private Button stampButton;
    [SerializeField] private Image stampImage;
    [SerializeField] private Text statusText;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color stampedColor = Color.green;

    [Header("Audio")]
    [SerializeField] private AudioSource stampSound;

    private bool isStampActive = false;

    private void Start()
    {
        if (stampButton != null)
        {
            stampButton.onClick.AddListener(ToggleStampMode);
        }

        UpdateVisuals();
    }

    private void Update()
    {
        // If stamp is active and player clicks a ticket, stamp it
        if (isStampActive && clickToStampMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckForTicketClick();
            }
        }
    }

    /// <summary>
    /// Toggle stamp active/inactive state
    /// </summary>
    public void ToggleStampMode()
    {
        isStampActive = !isStampActive;
        UpdateVisuals();

        if (isStampActive)
        {
            SystemLog.Instance?.LogMessage("Stamp ready - Click a ticket to mark complete");
        }
        else
        {
            SystemLog.Instance?.LogMessage("Stamp deactivated");
        }
    }

    /// <summary>
    /// Handle ticket being dropped on stamp
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Ticket ticket = droppedObject.GetComponent<Ticket>();

            if (ticket != null)
            {
                StampTicket(ticket);
            }
        }
    }

    /// <summary>
    /// Check if player clicked on a ticket while stamp is active
    /// </summary>
    private void CheckForTicketClick()
    {
        // Raycast to find what was clicked
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Ticket ticket = result.gameObject.GetComponent<Ticket>();

            if (ticket != null)
            {
                StampTicket(ticket);
                isStampActive = false; // Deactivate after stamping
                UpdateVisuals();
                break;
            }
        }
    }

    /// <summary>
    /// Stamp a ticket as completed
    /// </summary>
    public void StampTicket(Ticket ticket)
    {
        if (ticket == null) return;

        if (ticket.IsStamped())
        {
            SystemLog.Instance?.LogMessage("Ticket already stamped!");
            return;
        }

        ticket.StampTicket();

        // Visual feedback
        if (stampSound != null)
        {
            stampSound.Play();
        }

        // Flash stamp color
        StartCoroutine(StampFlash());

        SystemLog.Instance?.LogMessage($"Completed: {ticket.GetTaskTitle()}");
    }

    /// <summary>
    /// Visual feedback when stamping
    /// </summary>
    private System.Collections.IEnumerator StampFlash()
    {
        if (stampImage != null)
        {
            Color original = stampImage.color;
            stampImage.color = stampedColor;
            yield return new WaitForSeconds(0.2f);
            stampImage.color = original;
        }
    }

    /// <summary>
    /// Update stamp visual state
    /// </summary>
    private void UpdateVisuals()
    {
        if (stampImage != null)
        {
            stampImage.color = isStampActive ? activeColor : normalColor;
        }

        if (statusText != null)
        {
            statusText.text = isStampActive ? "ACTIVE" : "Ready";
        }
    }

    /// <summary>
    /// Manually stamp a specific ticket (called externally)
    /// </summary>
    public void StampTicketByReference(Ticket ticket)
    {
        StampTicket(ticket);
    }

    // Public getters
    public bool IsActive() => isStampActive;
}
