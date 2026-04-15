using UnityEngine;

public class GameService : MonoBehaviour
{
    [SerializeField] private float dayDuration = 90f;

    private GameState _gameState;
    private TaskService _taskService;
    private TicketService _ticketService;
    private MinigameFactory _minigameFactory;
    private DayService _dayService;
    private bool _isRunning = false;

    public GameState State => _gameState;
    public TaskService Tasks => _taskService;
    public TicketService Tickets => _ticketService;

    public void Initialize(GameState state, TaskService taskService, TicketService ticketService,
                          MinigameFactory minigameFactory, DayService dayService)
    {
        _gameState = state;
        _taskService = taskService;
        _ticketService = ticketService;
        _minigameFactory = minigameFactory;
        _dayService = dayService;

        // Subscribe to events
        GameEvents.Instance.OnTicketStamped.AddListener(OnTicketStamped);
        GameEvents.Instance.OnTicketShredded.AddListener(OnTicketShredded);
        GameEvents.Instance.OnDayEnded.AddListener(OnDayEnded);

        _isRunning = true;
    }

    private void Start()
    {
        StartNewDay();
    }

    private void Update()
    {
        if (!_isRunning) return;

        // Update day timer
        GameStateData updated = _gameState.Current.WithDayTimer(_gameState.Current.DayTimer + Time.deltaTime);
        _gameState.Update(updated);
        GameEvents.Instance?.OnDayTimerUpdated.Invoke(updated.DayTimer);

        // Check if day is over
        if (updated.DayTimer >= dayDuration)
        {
            _isRunning = false;
            EndDay();
        }
    }

    private void StartNewDay()
    {
        GameStateData newState = _gameState.Current
            .WithDay(_gameState.Current.CurrentDay)
            .WithTasksCompleted(0)
            .WithDayTimer(0f);
        _gameState.Update(newState);

        GameEvents.Instance?.OnDayStarted.Invoke();
        _isRunning = true;
    }

    private void EndDay()
    {
        GameEvents.Instance?.OnDayEnded.Invoke();

        if (_gameState.Current.TasksCompleted >= Constants.TASKS_REQUIRED_PER_DAY)
        {
            // WIN - advance to next day
            if (_gameState.Current.CurrentDay < Constants.DAY_COUNT)
            {
                GameStateData advanced = _gameState.Current
                    .WithDay(_gameState.Current.CurrentDay + 1)
                    .WithTasksCompleted(0)
                    .WithDayTimer(0f);
                _gameState.Update(advanced);
                GameEvents.Instance?.OnDayChanged.Invoke(advanced.CurrentDay);

                // Show upgrade modal
                _dayService.ShowUpgradeModal(_gameState.Current.CurrentDay - 1);
                StartNewDay();
            }
            else
            {
                // Day 5 WIN
                GameEvents.Instance?.OnGameWon.Invoke();
            }
        }
        else
        {
            // LOSE
            if (_gameState.Current.CurrentDay == Constants.DAY_COUNT)
            {
                GameEvents.Instance?.OnGameLost.Invoke();
            }
            else
            {
                // Retry or show failure screen
                _dayService.ShowDayFailure(_gameState.Current.CurrentDay);
                Invoke(nameof(StartNewDay), 2f);
            }
        }
    }

    public void OnTicketStamped(TicketModel ticket)
    {
        // Increment task counter
        GameStateData updated = _gameState.Current
            .WithTasksCompleted(_gameState.Current.TasksCompleted + 1);
        _gameState.Update(updated);
        GameEvents.Instance?.OnTasksCompletedChanged.Invoke(updated.TasksCompleted);
    }

    public void OnTicketShredded(TicketModel ticket)
    {
        // Award outputs
        GameStateData updated = _gameState.Current
            .WithOutputs(_gameState.Current.Outputs + ticket.TaskData.BaseReward);
        _gameState.Update(updated);
        GameEvents.Instance?.OnOutputsChanged.Invoke(updated.Outputs);
    }

    private void OnDayEnded()
    {
        _minigameFactory.Close();
    }
}
