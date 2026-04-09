using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// Printer that generates task tickets for the player.
/// Prints tickets at regular intervals or on demand.
/// </summary>
public class Printer : MonoBehaviour
{
    [Header("Printer Settings")]
    [SerializeField] private float autoPrintInterval = 15f; // Print every 15 seconds
    [SerializeField] private bool autoPrintEnabled = true;
    [SerializeField] private int maxActiveTickets = 10; // Limit active tickets

    [Header("Ticket Prefab")]
    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private Transform ticketSpawnPoint; // Where tickets appear

    [Header("UI References")]
    [SerializeField] private GameObject printerPanel; // Root visual panel — assign in Inspector
    [SerializeField] private Button printButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image printerAnimation; // Optional: visual feedback

    [Header("Audio")]
    [SerializeField] private AudioSource printSound;

    [Header("Board Animation")]
    [SerializeField] private BulletinBoard bulletinBoard; // Auto-discovered if not assigned
    [SerializeField] private float travelDuration = 0.8f;

    [Header("Bulletin Board Spawn Bounds")]
    [SerializeField] private Vector2 boardCenter = new Vector2(-418f, 180f);
    [SerializeField] private Vector2 boardSize = new Vector2(785f, 398f);

    private float timeSinceLastPrint = 0f;
    private int activeTicketCount = 0;
    private bool hasInitialized = false;

    private void Awake()
    {
        // Hide the printer panel on start so the scene view stays clean.
        // The root GameObject stays active so Update/auto-print still runs.
        // Assign 'printerPanel' in the Inspector to a child Panel wrapping all printer UI.
        if (printerPanel != null)
        {
            printerPanel.SetActive(false);
        }
    }

    private void Start()
    {
        // Ticket prefab is assigned by SetupAllAssets via reflection
        if (ticketPrefab == null)
        {
            Debug.LogError("Printer: TicketPrefab not assigned! Assign it in SetupAllAssets.");
            return;
        }

        // Auto-discover bulletin board if not assigned
        if (bulletinBoard == null)
        {
            bulletinBoard = FindFirstObjectByType<BulletinBoard>();
            if (bulletinBoard == null)
            {
                Debug.LogWarning("Printer: BulletinBoard not found in scene. Ticket animation will not work.");
            }
        }

        if (printButton != null)
        {
            printButton.onClick.AddListener(PrintTicket);
        }

        // Show the printer panel once the game is ready
        Show();

        // Print first ticket immediately on play (assume SetupAllAssets has finished)
        PrintTicket();
        timeSinceLastPrint = 0f;

        UpdateStatusUI();
    }

    /// <summary>
    /// Make the printer panel visible.
    /// </summary>
    public void Show()
    {
        if (printerPanel != null)
        {
            printerPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the printer panel.
    /// </summary>
    public void Hide()
    {
        if (printerPanel != null)
        {
            printerPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // First frame initialization (workaround for Start() not running on dynamically created components)
        if (!hasInitialized)
        {
            hasInitialized = true;
            PrintTicket();  // Print first ticket
        }

        if (autoPrintEnabled)
        {
            timeSinceLastPrint += Time.deltaTime;

            if (timeSinceLastPrint >= autoPrintInterval)
            {
                PrintTicket();
                timeSinceLastPrint = 0f;
            }
        }

        UpdateStatusUI();
    }

    /// <summary>
    /// Print a new ticket with a task
    /// </summary>
    public void PrintTicket()
    {
        // Check if we've hit the ticket limit
        if (activeTicketCount >= maxActiveTickets)
        {
            SystemLog.Instance?.LogMessage("Printer: Too many active tickets!");
            return;
        }

        // Get task from database
        if (TaskDatabase.Instance == null)
        {
            Debug.LogWarning("TaskDatabase not found!");
            return;
        }

        int currentDay = GameManager.Instance?.GetCurrentDay() ?? 1;
        TaskData task = TaskDatabase.Instance.GetRandomTaskForDay(currentDay);

        // Create ticket (use pool if available, otherwise instantiate)
        if (ticketPrefab != null)
        {
            GameObject ticketObj = TicketPool.Instance != null
                ? TicketPool.Instance.GetTicket()
                : Instantiate(ticketPrefab, GetComponentInParent<Canvas>()?.transform ?? transform.parent);

            // Ensure ticket renders on top by making it the last child
            ticketObj.transform.SetAsLastSibling();

            RectTransform ticketRect = ticketObj.GetComponent<RectTransform>();

            // Position it randomly within bulletin board bounds
            if (ticketRect != null)
            {
                // Set explicit size if not already set
                if (ticketRect.sizeDelta == Vector2.zero)
                {
                    ticketRect.sizeDelta = new Vector2(140, 77); // Match the ticket sprite dimensions
                }

                // Random position within board bounds
                float randomX = Random.Range(boardCenter.x - boardSize.x / 2f, boardCenter.x + boardSize.x / 2f);
                float randomY = Random.Range(boardCenter.y - boardSize.y / 2f, boardCenter.y + boardSize.y / 2f);
                ticketRect.anchoredPosition = new Vector2(randomX, randomY);

                // Spawn in mini mode (0.3x scale)
                ticketRect.localScale = new Vector3(0.3f, 0.3f, 1f);
            }

            Ticket ticket = ticketObj.GetComponent<Ticket>();

            if (ticket != null)
            {
                // Use new Initialize method that passes full TaskData (includes minigame info)
                ticket.Initialize(task);
                activeTicketCount++;

                SystemLog.Instance?.LogMessage($"Ticket printed: {task.title}");

                // Play print animation/sound
                StartCoroutine(PrintAnimation());

                if (printSound != null)
                {
                    printSound.Play();
                }

                // Animate ticket from printer to bulletin board
                if (bulletinBoard != null)
                {
                    StartCoroutine(AnimateTicketToBoard(ticket, ticketObj, ticketRect));
                }
            }
        }
        else
        {
            Debug.LogWarning("Printer: Ticket prefab or spawn point not assigned!");
        }
    }

    /// <summary>
    /// Visual feedback for printing
    /// </summary>
    private IEnumerator PrintAnimation()
    {
        if (printerAnimation != null)
        {
            // Simple flash effect
            Color originalColor = printerAnimation.color;
            printerAnimation.color = Color.yellow;
            yield return new WaitForSeconds(0.3f);
            printerAnimation.color = originalColor;
        }
    }

    /// <summary>
    /// Update printer status display
    /// </summary>
    private void UpdateStatusUI()
    {
        if (statusText != null)
        {
            float timeUntilNext = autoPrintInterval - timeSinceLastPrint;
            statusText.text = $"Next Ticket: {timeUntilNext:F0}s\nActive: {activeTicketCount}/{maxActiveTickets}";
        }

        // Disable button if at capacity
        if (printButton != null)
        {
            printButton.interactable = activeTicketCount < maxActiveTickets;
        }
    }

    /// <summary>
    /// Called when a ticket is processed/destroyed
    /// </summary>
    public void OnTicketProcessed()
    {
        activeTicketCount = Mathf.Max(0, activeTicketCount - 1);
    }

    /// <summary>
    /// Toggle auto-printing on/off
    /// </summary>
    public void ToggleAutoPrint()
    {
        autoPrintEnabled = !autoPrintEnabled;
        SystemLog.Instance?.LogMessage($"Auto-print: {(autoPrintEnabled ? "ON" : "OFF")}");
    }

    /// <summary>
    /// Animate ticket from printer spawn point to bulletin board slot
    /// </summary>
    private IEnumerator AnimateTicketToBoard(Ticket ticket, GameObject ticketObj, RectTransform rectTransform)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector3 miniScale = new Vector3(0.3f, 0.3f, 1f);

        // Get target position from bulletin board
        Vector2 targetPosition = startPosition;
        if (bulletinBoard != null)
        {
            // GetNextSlotAnchoredPosition returns local position relative to BulletinBoard
            // We need to convert it to Canvas space by adding the BulletinBoard's position
            Vector2 boardSlotLocalPosition = bulletinBoard.GetNextSlotAnchoredPosition();
            RectTransform boardRectTransform = bulletinBoard.GetComponent<RectTransform>();
            if (boardRectTransform != null)
            {
                targetPosition = boardRectTransform.anchoredPosition + boardSlotLocalPosition;
            }
            else
            {
                targetPosition = boardSlotLocalPosition;
            }
        }

        float elapsed = 0f;
        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / travelDuration;
            t = Mathf.SmoothStep(0, 1, t);

            // Lerp position from printer to board (using anchored position for UI)
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            // Keep mini scale during travel (ticket already spawned at 0.3x)
            rectTransform.localScale = miniScale;

            yield return null;
        }

        // Ensure final values
        rectTransform.anchoredPosition = targetPosition;
        rectTransform.localScale = miniScale;

        // Pin the ticket to the bulletin board
        if (bulletinBoard != null)
        {
            bulletinBoard.PinTicket(ticket);
        }
    }

    // Public getters/setters
    public void SetAutoPrintInterval(float interval) => autoPrintInterval = interval;
    public int GetActiveTicketCount() => activeTicketCount;
    public int GetMaxTickets() => maxActiveTickets;
}
