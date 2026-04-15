using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PrinterController : MonoBehaviour
{
    [SerializeField] private Button printButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image printerAnimation;
    [SerializeField] private Transform ticketSpawnPoint;

    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private Transform boardContainer;
    [SerializeField] private float printInterval = Constants.TICKET_SPAWN_INTERVAL;

    private TicketViewFactory _ticketFactory;
    private GameService _gameService;
    private MinigameFactory _minigameFactory;
    private float _timeSinceLastPrint = 0f;

    public void Initialize(GameService gameService, MinigameFactory minigameFactory)
    {
        _gameService = gameService;
        _minigameFactory = minigameFactory;
        _ticketFactory = new TicketViewFactory(ticketPrefab, boardContainer);

        if (printButton != null)
            printButton.onClick.AddListener(PrintTicket);

        // Subscribe to events
        GameEvents.Instance.OnDayStarted.AddListener(ResetTimer);

        PrintTicket(); // Print first ticket immediately
    }

    private void Update()
    {
        _timeSinceLastPrint += Time.deltaTime;

        if (_timeSinceLastPrint >= printInterval && _gameService.Tickets.HasRoom())
        {
            PrintTicket();
            _timeSinceLastPrint = 0f;
        }

        UpdateStatusUI();
    }

    private void PrintTicket()
    {
        if (!_gameService.Tickets.HasRoom())
        {
            Debug.LogWarning("PrinterController: Max active tickets reached");
            return;
        }

        TaskData task = _gameService.Tasks.GetRandomTaskForDay(_gameService.State.Current.CurrentDay);
        TicketModel ticket = _gameService.Tickets.CreateTicket(task);

        if (ticket == null) return;

        // Create view
        TicketView view = _ticketFactory.CreateTicketView(ticket, () =>
        {
            OnStartTask(ticket);
        });

        // Animate to board
        StartCoroutine(AnimateTicketToBoard(view));

        // Play animation
        StartCoroutine(PrintAnimation());

        GameEvents.Instance?.OnTicketSpawned.Invoke(ticket);
    }

    private void OnStartTask(TicketModel ticket)
    {
        if (!ticket.TaskData.RequiresMinigame)
            return;

        _minigameFactory.OpenMinigame(ticket.TaskData.MinigameType, ticket.TaskData.TypingTask, () =>
        {
            // Stamp the ticket
            _gameService.Tickets.StampTicket(ticket);
        });
    }

    private IEnumerator AnimateTicketToBoard(TicketView ticketView)
    {
        RectTransform rect = ticketView.GetComponent<RectTransform>();
        Vector2 startPos = ticketSpawnPoint.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = new Vector2(0, 0); // Board center

        float elapsed = 0f;
        while (elapsed < Constants.TICKET_TRAVEL_DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / Constants.TICKET_TRAVEL_DURATION;
            t = Mathf.SmoothStep(0, 1, t);

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            rect.localScale = Vector3.Lerp(Vector3.one * Constants.TICKET_MINI_SCALE, Vector3.one * Constants.TICKET_MINI_SCALE, t);

            yield return null;
        }

        rect.anchoredPosition = endPos;
        rect.localScale = Vector3.one * Constants.TICKET_MINI_SCALE;
    }

    private IEnumerator PrintAnimation()
    {
        if (printerAnimation != null)
        {
            Color original = printerAnimation.color;
            printerAnimation.color = Color.yellow;
            yield return new WaitForSeconds(0.3f);
            printerAnimation.color = original;
        }
    }

    private void ResetTimer()
    {
        _timeSinceLastPrint = 0f;
    }

    private void UpdateStatusUI()
    {
        if (statusText != null)
        {
            float timeUntilNext = printInterval - _timeSinceLastPrint;
            statusText.text = $"Next: {timeUntilNext:F0}s | Active: {_gameService.Tickets.GetActiveCount()}/{Constants.MAX_ACTIVE_TICKETS}";
        }

        if (printButton != null)
            printButton.interactable = _gameService.Tickets.HasRoom();
    }
}
