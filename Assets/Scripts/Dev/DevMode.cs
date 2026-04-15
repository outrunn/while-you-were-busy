using UnityEngine;
using TMPro;

public class DevMode : MonoBehaviour
{
    private static DevMode _instance;
    public static bool AutoWinEnabled { get; private set; } = false;

    private GameService _gameService;
    private GameState _gameState;
    private bool _devPanelVisible = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _gameService = FindFirstObjectByType<GameService>();
        if (_gameService != null)
        {
            _gameState = _gameService.State;
        }

        Debug.Log("[DevMode] Loaded. Press 'D' to toggle debug panel.");
        Debug.Log("  [1] +1 Task | [2] Instant Win | [3] Instant Lose");
        Debug.Log("  [4] Skip to Day 5 | [5] Toggle Slow-Mo | [6] Spawn 10 Tickets");
        Debug.Log("  [7] Print State | [8] Reset Game");
    }

    private void Update()
    {
        // Toggle dev panel with 'D'
        if (Input.GetKeyDown(KeyCode.D))
        {
            _devPanelVisible = !_devPanelVisible;
            string state = _devPanelVisible ? "ON" : "OFF";
            Debug.Log($"[DevMode] Panel {state}");
        }

        // Old auto-win toggle (= key)
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AutoWinEnabled = !AutoWinEnabled;
            Debug.Log($"[DevMode] Auto-win mode: {(AutoWinEnabled ? "ENABLED" : "DISABLED")}");
        }

        if (!_devPanelVisible) return;

        // Debug commands with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AddTask();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            InstantWin();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            InstantLose();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SkipToDay5();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            ToggleTimeScale();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SpawnMultipleTickets(10);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            PrintGameState();

        if (Input.GetKeyDown(KeyCode.Alpha8))
            ResetGame();
    }

    private void AddTask()
    {
        if (_gameState == null) return;

        GameStateData updated = _gameState.Current
            .WithTasksCompleted(_gameState.Current.TasksCompleted + 1);
        _gameState.Update(updated);

        Debug.Log($"[DevMode] +1 Task | Total: {updated.TasksCompleted}/4");
        GameEvents.Instance?.OnTasksCompletedChanged.Invoke(updated.TasksCompleted);
    }

    private void InstantWin()
    {
        if (_gameState == null) return;

        int tasksNeeded = Constants.TASKS_REQUIRED_PER_DAY;
        GameStateData updated = _gameState.Current.WithTasksCompleted(tasksNeeded);
        _gameState.Update(updated);

        Debug.Log($"[DevMode] Day {_gameState.Current.CurrentDay} WIN - advancing");
        GameEvents.Instance?.OnTasksCompletedChanged.Invoke(tasksNeeded);

        if (_gameService != null)
        {
            _gameService.GetType().GetMethod("EndDay",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_gameService, null);
        }
    }

    private void InstantLose()
    {
        if (_gameState == null) return;

        GameStateData updated = _gameState.Current.WithDayTimer(90f);
        _gameState.Update(updated);

        Debug.Log($"[DevMode] Day {_gameState.Current.CurrentDay} LOSE");

        if (_gameService != null)
        {
            _gameService.GetType().GetMethod("EndDay",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_gameService, null);
        }
    }

    private void SkipToDay5()
    {
        if (_gameState == null) return;

        GameStateData updated = _gameState.Current
            .WithDay(5)
            .WithTasksCompleted(0)
            .WithDayTimer(0f);
        _gameState.Update(updated);

        Debug.Log("[DevMode] Skipped to Day 5");
        GameEvents.Instance?.OnDayChanged.Invoke(5);
    }

    private void ToggleTimeScale()
    {
        Time.timeScale = Time.timeScale == 1f ? 0.5f : 1f;
        Debug.Log($"[DevMode] Time Scale: {Time.timeScale}x");
    }

    private void SpawnMultipleTickets(int count)
    {
        GameObject ticketPrefab = Resources.Load<GameObject>("Prefabs/Tickets/Ticket");
        if (ticketPrefab == null)
        {
            Debug.LogWarning("[DevMode] Ticket prefab not found");
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[DevMode] Canvas not found");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-400f, 400f),
                Random.Range(-300f, 300f),
                0
            );

            GameObject ticket = Instantiate(ticketPrefab, canvas.transform);
            RectTransform rect = ticket.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = randomPos;
            }
        }

        Debug.Log($"[DevMode] Spawned {count} tickets");
    }

    private void PrintGameState()
    {
        if (_gameState == null)
        {
            Debug.Log("[DevMode] GameState is null");
            return;
        }

        var current = _gameState.Current;
        Debug.Log($"=== GAME STATE ===\nDay: {current.CurrentDay}/5\nTasks: {current.TasksCompleted}/{Constants.TASKS_REQUIRED_PER_DAY}\nTimer: {current.DayTimer:F1}s / 90s\nHealth: {current.WorldHealth}\nOutputs: {current.Outputs}\n==================");
    }

    private void ResetGame()
    {
        if (_gameState == null) return;

        _gameState.Reset();
        Debug.Log("[DevMode] Game reset to Day 1");
        GameEvents.Instance?.OnDayChanged.Invoke(1);
    }
}
