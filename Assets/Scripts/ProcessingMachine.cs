using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// Processing machine that accepts stamped tickets and grants rewards.
/// Players drag stamped tickets here to complete tasks and earn outputs.
/// </summary>
public class ProcessingMachine : MonoBehaviour, IDropHandler
{
    [Header("Machine Settings")]
    [SerializeField] private float processingTime = 2f; // Time to process a ticket
    [SerializeField] private bool requireStampedTickets = true;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image machineImage;
    [SerializeField] private Image progressBar; // Image with fillAmount for progress

    [Header("Visual Feedback")]
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private Color processingColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource processingSound;
    [SerializeField] private AudioSource completeSound;

    [Header("References")]
    [SerializeField] private Printer printer; // To notify when ticket is processed

    private bool isProcessing = false;
    private Ticket currentTicket = null;

    private void Start()
    {
        // Auto-find Printer if not assigned — prevents activeTicketCount from getting stuck
        if (printer == null)
        {
            printer = FindObjectOfType<Printer>();
            if (printer == null)
            {
                Debug.LogWarning("ProcessingMachine: No Printer found in scene. Processed tickets won't decrement the active ticket count.");
            }
        }

        UpdateVisuals();

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Handle ticket being dropped on processing machine
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            Ticket ticket = droppedObject.GetComponent<Ticket>();

            if (ticket != null)
            {
                ProcessTicket(ticket);
            }
        }
    }

    /// <summary>
    /// Process a ticket for rewards
    /// </summary>
    public void ProcessTicket(Ticket ticket)
    {
        if (isProcessing)
        {
            SystemLog.Instance?.LogMessage("Machine busy - Please wait");
            ticket.ResetPosition();
            return;
        }

        // Check if ticket is stamped (if required)
        if (requireStampedTickets && !ticket.IsStamped())
        {
            SystemLog.Instance?.LogMessage("Error: Ticket not stamped!");
            StartCoroutine(ErrorFlash());
            ticket.ResetPosition();
            return;
        }

        // Start processing
        currentTicket = ticket;
        StartCoroutine(ProcessingRoutine(ticket));
    }

    /// <summary>
    /// Processing animation and reward granting
    /// </summary>
    private IEnumerator ProcessingRoutine(Ticket ticket)
    {
        if (ticket == null)
        {
            isProcessing = false;
            yield break;
        }

        isProcessing = true;
        float elapsed = 0f;

        // Show progress bar
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.fillAmount = 0f;
        }

        // Change machine color
        if (machineImage != null)
        {
            machineImage.color = processingColor;
        }

        // Play processing sound
        if (processingSound != null && !processingSound.isPlaying)
        {
            processingSound.Play();
        }

        // Update status
        if (statusText != null)
        {
            statusText.text = $"Processing:\n{ticket.GetTaskTitle()}";
        }

        // Hide the ticket visually during processing
        CanvasGroup ticketCanvas = ticket.GetComponent<CanvasGroup>();
        if (ticketCanvas != null)
        {
            ticketCanvas.alpha = 0.3f;
        }

        // Processing animation
        while (elapsed < processingTime)
        {
            elapsed += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.fillAmount = elapsed / processingTime;
            }

            yield return null;
        }

        // Processing complete - Grant reward
        GrantReward(ticket);

        // Play completion sound
        if (completeSound != null)
        {
            completeSound.Play();
        }

        // Destroy the ticket
        if (printer != null)
        {
            printer.OnTicketProcessed();
        }

        Destroy(ticket.gameObject);

        // Reset machine
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        if (machineImage != null)
        {
            machineImage.color = idleColor;
        }

        if (statusText != null)
        {
            statusText.text = "Ready";
        }

        isProcessing = false;
        currentTicket = null;
    }

    /// <summary>
    /// Grant rewards from ticket to player and degrade world health
    /// </summary>
    private void GrantReward(Ticket ticket)
    {
        if (GameManager.Instance != null)
        {
            float reward = ticket.GetReward();

            // Add bonus based on difficulty
            float difficultyMultiplier = 1f + (ticket.GetDifficulty() - 1) * 0.5f;
            float finalReward = reward * difficultyMultiplier;

            // Grant the reward
            GameManager.Instance.AddOutputs(finalReward);

            // Degrade world health based on task difficulty
            float healthCost = ticket.GetDifficulty() * 0.5f;
            GameManager.Instance.DegradeWorldHealth(healthCost);

            SystemLog.Instance?.LogMessage($"Task complete! +{finalReward:F0} outputs");
        }
    }

    /// <summary>
    /// Flash error color
    /// </summary>
    private IEnumerator ErrorFlash()
    {
        if (machineImage != null)
        {
            Color original = machineImage.color;
            machineImage.color = errorColor;
            yield return new WaitForSeconds(0.3f);
            machineImage.color = original;
        }
    }

    /// <summary>
    /// Update machine visual state
    /// </summary>
    private void UpdateVisuals()
    {
        if (machineImage != null && !isProcessing)
        {
            machineImage.color = idleColor;
        }

        if (statusText != null && !isProcessing)
        {
            statusText.text = "Ready";
        }
    }

    // Public getters
    public bool IsProcessing() => isProcessing;
    public Ticket GetCurrentTicket() => currentTicket;
}
