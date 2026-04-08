using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Bulletin board where players can pin tickets to track active tasks.
/// Acts as a drop zone for ticket organization.
/// </summary>
public class BulletinBoard : MonoBehaviour, IDropHandler
{
    [Header("Board Settings")]
    [SerializeField] private int maxPinnedTickets = 10;
    [SerializeField] private Transform ticketContainer; // Where pinned tickets go

    [Header("Layout Settings")]
    [SerializeField] private float ticketSpacing = 10f;
    [SerializeField] private int ticketsPerRow = 3;
    [SerializeField] private Vector2 ticketSize = new Vector2(150, 100);

    private List<Ticket> pinnedTickets = new List<Ticket>();

    private void Start()
    {
        if (ticketContainer == null)
        {
            ticketContainer = transform;
        }
    }

    /// <summary>
    /// Handle ticket being dropped on bulletin board
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Ticket ticket = droppedObject.GetComponent<Ticket>();

            if (ticket != null && !ticket.IsPinned())
            {
                PinTicket(ticket);
            }
        }
    }

    /// <summary>
    /// Pin a ticket to the bulletin board
    /// </summary>
    public void PinTicket(Ticket ticket)
    {
        if (pinnedTickets.Count >= maxPinnedTickets)
        {
            SystemLog.Instance?.LogMessage("Bulletin Board: No more room!");
            ticket.ResetPosition();
            return;
        }

        if (!pinnedTickets.Contains(ticket))
        {
            pinnedTickets.Add(ticket);
            ticket.Pin(ticketContainer);
            OrganizeTickets();

            SystemLog.Instance?.LogMessage($"Pinned: {ticket.GetTaskTitle()}");
        }
    }

    /// <summary>
    /// Remove ticket from bulletin board
    /// </summary>
    public void UnpinTicket(Ticket ticket)
    {
        if (pinnedTickets.Contains(ticket))
        {
            pinnedTickets.Remove(ticket);
            ticket.Unpin();
            OrganizeTickets();

            SystemLog.Instance?.LogMessage($"Unpinned: {ticket.GetTaskTitle()}");
        }
    }

    /// <summary>
    /// Organize tickets in a grid layout
    /// </summary>
    private void OrganizeTickets()
    {
        for (int i = 0; i < pinnedTickets.Count; i++)
        {
            if (pinnedTickets[i] == null) continue;

            RectTransform rectTransform = pinnedTickets[i].GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                // Calculate grid position
                int row = i / ticketsPerRow;
                int col = i % ticketsPerRow;

                float xPos = col * (ticketSize.x + ticketSpacing);
                float yPos = -row * (ticketSize.y + ticketSpacing);

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);
            }
        }
    }

    /// <summary>
    /// Get all pinned tickets
    /// </summary>
    public List<Ticket> GetPinnedTickets()
    {
        // Remove any null references
        pinnedTickets.RemoveAll(ticket => ticket == null);
        return pinnedTickets;
    }

    /// <summary>
    /// Get all stamped tickets on the board
    /// </summary>
    public List<Ticket> GetStampedTickets()
    {
        List<Ticket> stampedTickets = new List<Ticket>();

        foreach (Ticket ticket in pinnedTickets)
        {
            if (ticket != null && ticket.IsStamped())
            {
                stampedTickets.Add(ticket);
            }
        }

        return stampedTickets;
    }

    /// <summary>
    /// Clear all tickets from board
    /// </summary>
    public void ClearBoard()
    {
        foreach (Ticket ticket in pinnedTickets)
        {
            if (ticket != null)
            {
                Destroy(ticket.gameObject);
            }
        }

        pinnedTickets.Clear();
    }

    /// <summary>
    /// Get the world position where the next ticket will be pinned
    /// </summary>
    public Vector3 GetNextSlotWorldPosition()
    {
        int nextIndex = pinnedTickets.Count;
        int row = nextIndex / ticketsPerRow;
        int col = nextIndex % ticketsPerRow;

        float xPos = col * (ticketSize.x + ticketSpacing);
        float yPos = -row * (ticketSize.y + ticketSpacing);

        Vector2 localPos = new Vector2(xPos, yPos);

        if (ticketContainer != null && ticketContainer is RectTransform rectTransform)
        {
            return rectTransform.TransformPoint(localPos);
        }

        return Vector3.zero;
    }

    // Public getters
    public int GetPinnedCount() => pinnedTickets.Count;
    public int GetMaxCapacity() => maxPinnedTickets;
    public bool HasRoom() => pinnedTickets.Count < maxPinnedTickets;
}
