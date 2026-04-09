using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pool for ticket prefabs to reduce GC allocations and improve performance.
/// Instead of Instantiate/Destroy, reuses ticket instances.
/// </summary>
public class TicketPool : MonoBehaviour
{
    public static TicketPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private int initialPoolSize = 15; // 6 tickets/day * 5 days + buffer
    [SerializeField] private Transform ticketParent;

    private Queue<GameObject> availableTickets = new Queue<GameObject>();
    private HashSet<GameObject> activeTickets = new HashSet<GameObject>();

    private void Awake()
    {
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
        InitializePool();
    }

    /// <summary>
    /// Pre-create all pooled tickets
    /// </summary>
    private void InitializePool()
    {
        if (ticketPrefab == null)
        {
            Debug.LogError("TicketPool: Ticket prefab not assigned!");
            return;
        }

        if (ticketParent == null)
        {
            // Default to root Canvas if parent not assigned
            Canvas rootCanvas = FindFirstObjectByType<Canvas>();
            ticketParent = rootCanvas != null ? rootCanvas.transform : transform.parent;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject ticket = Instantiate(ticketPrefab, ticketParent);
            ticket.SetActive(false);
            availableTickets.Enqueue(ticket);
        }

        Debug.Log($"TicketPool initialized with {initialPoolSize} tickets");
    }

    /// <summary>
    /// Get a ticket from the pool (or create one if pool is empty)
    /// </summary>
    public GameObject GetTicket()
    {
        GameObject ticket;

        if (availableTickets.Count > 0)
        {
            ticket = availableTickets.Dequeue();
        }
        else
        {
            // Emergency fallback: create a new ticket if pool is exhausted
            ticket = Instantiate(ticketPrefab, ticketParent);
            Debug.LogWarning("TicketPool exhausted! Creating new ticket. Consider increasing initialPoolSize.");
        }

        ticket.SetActive(true);
        activeTickets.Add(ticket);

        return ticket;
    }

    /// <summary>
    /// Return a ticket to the pool for reuse
    /// </summary>
    public void ReturnTicket(GameObject ticket)
    {
        if (ticket == null) return;

        // Reset ticket state
        ticket.SetActive(false);

        // Reset transform
        RectTransform rectTransform = ticket.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.SetParent(ticketParent);
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        // Reset components
        Ticket ticketComponent = ticket.GetComponent<Ticket>();
        if (ticketComponent != null)
        {
            ticketComponent.ResetForReuse();
        }

        activeTickets.Remove(ticket);
        availableTickets.Enqueue(ticket);
    }

    /// <summary>
    /// Get current pool statistics
    /// </summary>
    public (int available, int active, int total) GetPoolStats()
    {
        return (availableTickets.Count, activeTickets.Count, availableTickets.Count + activeTickets.Count);
    }

    /// <summary>
    /// Clear the pool (useful for scene cleanup)
    /// </summary>
    public void ClearPool()
    {
        foreach (var ticket in activeTickets)
        {
            if (ticket != null)
                Destroy(ticket);
        }

        foreach (var ticket in availableTickets)
        {
            if (ticket != null)
                Destroy(ticket);
        }

        activeTickets.Clear();
        availableTickets.Clear();
    }
}
