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
    [SerializeField] private float autoPrintInterval = 30f; // Print every 30 seconds
    [SerializeField] private bool autoPrintEnabled = true;
    [SerializeField] private int maxActiveTickets = 5; // Limit active tickets

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

    private float timeSinceLastPrint = 0f;
    private int activeTicketCount = 0;

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
        if (printButton != null)
        {
            printButton.onClick.AddListener(PrintTicket);
        }

        // Show the printer panel once the game is ready
        Show();

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

        int difficulty = TaskDatabase.Instance.GetDifficultyForCurrentProgress();
        TaskData task = TaskDatabase.Instance.GetRandomTask(difficulty);

        // Create ticket
        if (ticketPrefab != null && ticketSpawnPoint != null)
        {
            // Find the root Canvas so tickets are draggable across the whole screen
            Canvas rootCanvas = GetComponentInParent<Canvas>();
            Transform spawnParent = rootCanvas != null ? rootCanvas.transform : ticketSpawnPoint.parent;

            GameObject ticketObj = Instantiate(ticketPrefab, spawnParent);
            RectTransform ticketRect = ticketObj.GetComponent<RectTransform>();

            // Position it at the spawn point
            if (ticketRect != null)
            {
                ticketRect.position = ticketSpawnPoint.position;
                ticketRect.localScale = Vector3.one; // Ensure proper scale
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

    // Public getters/setters
    public void SetAutoPrintInterval(float interval) => autoPrintInterval = interval;
    public int GetActiveTicketCount() => activeTicketCount;
    public int GetMaxTickets() => maxActiveTickets;
}
