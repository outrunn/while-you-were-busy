# Architecture Rebuild — Implementation Plan

> **For agentic workers:** Use superpowers:subagent-driven-development to implement this plan task-by-task with review checkpoints.

**Goal:** Rebuild the game with clean, singleton-free architecture enabling scalability to 10+ minigames and seamless team onboarding.

**Architecture:**
- **Core State:** GameState (immutable data holder) + GameEvents (pub-sub system)
- **Services:** GameService, TaskService, MinigameService, TicketService (no singletons)
- **UI:** Clean MVVM separation — Views only render, Controllers handle input
- **Minigames:** MinigameFactory pattern — each minigame is a pluggable prefab with IMinigame interface
- **Initialization:** Dependency injection via single Bootstrapper, all UI at edit-time (no runtime creation)

**Tech Stack:** Unity 6, C# 11, TextMeshPro, ScriptableObjects for task data

---

## File Structure

### Core Architecture
```
Assets/Scripts/
├── Core/
│   ├── GameState.cs                    # Immutable state holder
│   ├── GameEvents.cs                   # Event channel (pub-sub)
│   ├── Bootstrapper.cs                 # Single entry point, injects dependencies
│   └── Constants.cs                    # All magic numbers here
├── Services/
│   ├── GameService.cs                  # Main game loop orchestration
│   ├── TaskService.cs                  # Task selection + pooling (replaces TaskDatabase)
│   ├── MinigameService.cs              # Minigame lifecycle management
│   ├── TicketService.cs                # Ticket lifecycle (no drag-drop logic)
│   └── DayService.cs                   # Day transitions + upgrades (split from GameManager)
├── Models/
│   ├── TaskData.cs                     # Cleaned up from Data/TaskDatabase.cs
│   ├── TypingTaskData.cs               # Split from TypingTaskDatabase
│   ├── GameStateData.cs                # Day, quota, timer (replaces GameManager fields)
│   └── TicketModel.cs                  # Pure data, no MonoBehaviour
├── UI/
│   ├── Controllers/
│   │   ├── GameHUDController.cs        # Manages GameHUD view
│   │   ├── UpgradeModalController.cs   # Manages upgrade modal
│   │   └── EndingController.cs         # Manages ending sequence
│   ├── Views/
│   │   ├── GameHUDView.cs              # Renders timer, quota, day text
│   │   ├── TicketView.cs               # Renders a single ticket
│   │   ├── UpgradeModalView.cs         # Renders upgrade options
│   │   └── EndingView.cs               # Renders ending typewriter
│   └── Utilities/
│       ├── TicketViewFactory.cs        # Creates TicketView instances
│       └── UIInteractionHandler.cs     # Centralized input handler
├── Minigames/
│   ├── IMinigame.cs                    # Interface all minigames implement
│   ├── MinigameFactory.cs              # Creates minigames by type (replaces MinigameManager)
│   ├── Typing/
│   │   ├── TypingMinigame.cs           # Implements IMinigame
│   │   └── TypingMinigameView.cs       # UI for typing
│   ├── Math/
│   │   ├── MathMinigame.cs
│   │   └── MathMinigameView.cs
│   ├── MultipleChoice/
│   │   ├── MultipleChoiceMinigame.cs
│   │   └── MultipleChoiceMinigameView.cs
│   ├── PhotoReveal/
│   │   ├── PhotoRevealMinigame.cs
│   │   └── PhotoRevealMinigameView.cs
│   └── Base/
│       └── BaseMinigame.cs             # Shared lifecycle logic
├── Utils/
│   ├── SystemLog.cs                    # Keep as-is
│   └── ObjectPool.cs                   # Generic pooling utility
└── Editor/
    └── MinigameSceneValidator.cs       # Validates scene setup at edit time
```

### Scene Structure
```
GameScene.unity
├── Canvas (ScreenSpaceOverlay)
│   ├── Background (Image: wallpaper)
│   ├── GameHUD (Panel for timer, quota, day)
│   ├── TicketBoard (Parent for all TicketViews)
│   ├── Printer (Parent for printer visual)
│   ├── Shredder (Parent for shredder visual)
│   ├── MinigameContainer (Parent for active minigame UI)
│   ├── ModalContainer (Parent for modals)
│   └── Overlays (Parent for tooltips, etc)
├── Bootstrapper (GameObject with Bootstrapper.cs)
├── [Minigame Prefabs — loaded on demand]
│   ├── Prefabs/TypingMinigame.prefab
│   ├── Prefabs/MathMinigame.prefab
│   ├── Prefabs/MultipleChoiceMinigame.prefab
│   └── Prefabs/PhotoRevealMinigame.prefab
```

---

## Task Breakdown

### Phase 1: Core Infrastructure (Days 1-2)

#### Task 1: GameState & Constants

**Files:**
- Create: `Assets/Scripts/Core/GameState.cs`
- Create: `Assets/Scripts/Core/Constants.cs`

- [ ] **Step 1: Create Constants.cs with all magic numbers**

```csharp
public static class Constants
{
    // Day/Time
    public const int DAY_COUNT = 5;
    public const float DAY_DURATION = 90f; // seconds
    public const float START_TIME = 8f; // 8:00 AM
    public const float END_TIME = 20f; // 8:00 PM

    // Quota
    public const int TASKS_REQUIRED_PER_DAY = 4;

    // Printer
    public const float TICKET_SPAWN_INTERVAL = 15f;
    public const int MAX_ACTIVE_TICKETS = 10;

    // Tickets
    public const float TICKET_MINI_SCALE = 0.3f;
    public const float TICKET_EXPAND_DURATION = 0.3f;
    public const float TICKET_TRAVEL_DURATION = 0.8f;

    // Health
    public const float INITIAL_WORLD_HEALTH = 100f;
    public const float HEALTH_DECAY_PER_CLICK = 0.1f;

    // Scaling
    public const float CLICK_POWER_MULTIPLIER = 2f;
}
```

- [ ] **Step 2: Create GameStateData.cs (immutable state holder)**

```csharp
[System.Serializable]
public class GameStateData
{
    public int CurrentDay { get; private set; }
    public int TasksCompleted { get; private set; }
    public float DayTimer { get; private set; }
    public float WorldHealth { get; private set; }
    public float Outputs { get; private set; }

    public GameStateData()
    {
        CurrentDay = 1;
        TasksCompleted = 0;
        DayTimer = 0f;
        WorldHealth = Constants.INITIAL_WORLD_HEALTH;
        Outputs = 0f;
    }

    // Return new instance with updated values (immutable pattern)
    public GameStateData WithDay(int day) => new()
    {
        CurrentDay = day,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithTasksCompleted(int count) => new()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = count,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithDayTimer(float timer) => new()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = timer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithWorldHealth(float health) => new()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = Mathf.Max(0, health),
        Outputs = Outputs
    };

    public GameStateData WithOutputs(float outputs) => new()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = outputs
    };
}

[System.Serializable]
public class GameState
{
    private GameStateData _data = new();

    public GameStateData Current => _data;

    public void Update(GameStateData newData) => _data = newData;

    public void Reset() => _data = new();
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Core/GameState.cs Assets/Scripts/Core/Constants.cs
git commit -m "feat: add game state and constants"
```

---

#### Task 2: Event System (GameEvents)

**Files:**
- Create: `Assets/Scripts/Core/GameEvents.cs`

- [ ] **Step 1: Create event system using UnityEvent**

```csharp
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    // Game state changes
    public UnityEvent<int> OnDayChanged = new();
    public UnityEvent<int> OnTasksCompletedChanged = new();
    public UnityEvent<float> OnDayTimerUpdated = new();
    public UnityEvent<float> OnWorldHealthChanged = new();
    public UnityEvent<float> OnOutputsChanged = new();

    // Day lifecycle
    public UnityEvent OnDayStarted = new();
    public UnityEvent OnDayEnded = new();
    public UnityEvent OnGameWon = new();
    public UnityEvent OnGameLost = new();

    // Ticket lifecycle
    public UnityEvent<TicketModel> OnTicketSpawned = new();
    public UnityEvent<TicketModel> OnTicketStamped = new();
    public UnityEvent<TicketModel> OnTicketShredded = new();

    // Minigame lifecycle
    public UnityEvent<MinigameType> OnMinigameStarted = new();
    public UnityEvent<MinigameType> OnMinigameCompleted = new();
    public UnityEvent<MinigameType> OnMinigameFailed = new();

    // Upgrade
    public UnityEvent<int> OnUpgradeApplied = new();

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
}

public enum MinigameType { None, Typing, Math, MultipleChoice, PhotoReveal }
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Core/GameEvents.cs
git commit -m "feat: add event system"
```

---

#### Task 3: Task Model & Service

**Files:**
- Create: `Assets/Scripts/Models/TaskData.cs`
- Create: `Assets/Scripts/Services/TaskService.cs`

- [ ] **Step 1: Create cleaned TaskData model**

```csharp
[System.Serializable]
public class TaskData
{
    public string Title;
    public string Description;
    public float BaseReward;
    public MinigameType MinigameType;

    // For typing tasks specifically
    public TypingTaskSO TypingTask;

    public bool RequiresMinigame => MinigameType != MinigameType.None;

    public TaskData(string title, string description, float reward, MinigameType type)
    {
        Title = title;
        Description = description;
        BaseReward = reward;
        MinigameType = type;
        TypingTask = null;
    }

    public TaskData(string title, string description, float reward, TypingTaskSO typingTask)
    {
        Title = title;
        Description = description;
        BaseReward = reward;
        MinigameType = typingTask != null ? MinigameType.Typing : MinigameType.None;
        TypingTask = typingTask;
    }
}
```

- [ ] **Step 2: Create TaskService (replaces TaskDatabase)**

```csharp
using System.Collections.Generic;
using UnityEngine;

public class TaskService
{
    private List<TaskData> _easyTasks = new();
    private List<TaskData> _mediumTasks = new();
    private List<TaskData> _hardTasks = new();
    private TypingTaskDatabase _typingTaskDb;

    public TaskService(TypingTaskDatabase typingTaskDb)
    {
        _typingTaskDb = typingTaskDb;
        InitializeDefaultTasks();
    }

    private void InitializeDefaultTasks()
    {
        _easyTasks.Clear();
        _mediumTasks.Clear();
        _hardTasks.Clear();

        // === TYPING TASKS ===
        AddTypingTask(_easyTasks, "Draft Status Email", "Compose a brief status update", 15f);
        AddTypingTask(_easyTasks, "Quick System Update", "Send quick notification", 12f);
        AddTypingTask(_easyTasks, "Meeting Confirmation", "Confirm meeting attendance", 10f);
        AddTypingTask(_easyTasks, "Daily Log Entry", "Record daily activities", 12f);
        AddTypingTask(_easyTasks, "Backup Confirmation", "Confirm backup completion", 10f);
        AddTypingTask(_mediumTasks, "Security Protocol Update", "Document new security measures", 65f);
        AddTypingTask(_mediumTasks, "Meeting Minutes", "Record system status meeting", 60f);
        AddTypingTask(_mediumTasks, "Performance Report", "Draft quarterly performance analysis", 70f);
        AddTypingTask(_hardTasks, "Incident Report", "File detailed incident analysis", 250f);
        AddTypingTask(_hardTasks, "System Architecture Document", "Draft comprehensive technical documentation", 280f);
        AddTypingTask(_hardTasks, "Executive Summary", "Prepare executive briefing", 300f);

        // === MULTIPLE CHOICE TASKS ===
        AddMultipleChoiceTask(_easyTasks, "File Format Question", "Which format is read-only?", 50f);
        AddMultipleChoiceTask(_easyTasks, "Storage Question", "What is the backup size limit?", 45f);
        AddMultipleChoiceTask(_mediumTasks, "Protocol Question", "Which protocol is most secure?", 200f);
        AddMultipleChoiceTask(_mediumTasks, "Storage Limit Question", "Maximum file size per upload?", 190f);
        AddMultipleChoiceTask(_hardTasks, "Authentication Question", "How many login attempts allowed?", 210f);

        // === MATH TASKS ===
        AddMathTask(_easyTasks, "Addition Problem", "Solve the math problem", 220f);
        AddMathTask(_easyTasks, "Subtraction Problem", "Solve the math problem", 230f);
        AddMathTask(_mediumTasks, "Mixed Math", "Solve the math problem", 240f);
        AddMathTask(_mediumTasks, "Complex Arithmetic", "Solve the math problem", 250f);

        // === PHOTO REVEAL TASKS ===
        AddPhotoRevealTask(_easyTasks, "Identify Object", "Hover to reveal the image", 280f);
        AddPhotoRevealTask(_mediumTasks, "Mosaic Puzzle", "Hover to reveal the hidden image", 290f);
        AddPhotoRevealTask(_hardTasks, "Blur Challenge", "Hover to sharpen the image", 300f);
    }

    private void AddTypingTask(List<TaskData> pool, string title, string description, float reward)
    {
        TypingTaskSO typingTask = _typingTaskDb?.GetRandomTypingTask();
        TaskData task = new(title, description, reward, typingTask);
        pool.Add(task);
    }

    private void AddMultipleChoiceTask(List<TaskData> pool, string title, string description, float reward)
    {
        pool.Add(new TaskData(title, description, reward, MinigameType.MultipleChoice));
    }

    private void AddMathTask(List<TaskData> pool, string title, string description, float reward)
    {
        pool.Add(new TaskData(title, description, reward, MinigameType.Math));
    }

    private void AddPhotoRevealTask(List<TaskData> pool, string title, string description, float reward)
    {
        pool.Add(new TaskData(title, description, reward, MinigameType.PhotoReveal));
    }

    public TaskData GetRandomTaskForDay(int day)
    {
        List<TaskData> allTasks = new();
        allTasks.AddRange(_easyTasks);
        allTasks.AddRange(_mediumTasks);
        allTasks.AddRange(_hardTasks);

        List<TaskData> validTasks = new();
        foreach (TaskData task in allTasks)
        {
            bool isValid = day switch
            {
                1 => task.MinigameType == MinigameType.Typing,
                2 => task.MinigameType == MinigameType.Typing || task.MinigameType == MinigameType.MultipleChoice,
                3 => task.MinigameType == MinigameType.Typing || task.MinigameType == MinigameType.MultipleChoice || task.MinigameType == MinigameType.Math,
                4 => task.MinigameType != MinigameType.None,
                5 => task.MinigameType == MinigameType.Typing,
                _ => false
            };

            if (isValid) validTasks.Add(task);
        }

        if (validTasks.Count > 0)
            return validTasks[Random.Range(0, validTasks.Count)];

        return GetRandomTask();
    }

    public TaskData GetRandomTask()
    {
        List<TaskData> allTasks = new();
        allTasks.AddRange(_easyTasks);
        allTasks.AddRange(_mediumTasks);
        allTasks.AddRange(_hardTasks);

        if (allTasks.Count > 0)
            return allTasks[Random.Range(0, allTasks.Count)];

        return new TaskData("Generic Task", "Process outputs efficiently", 10f, MinigameType.None);
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Models/TaskData.cs Assets/Scripts/Services/TaskService.cs
git commit -m "feat: add task model and service (replaces TaskDatabase)"
```

---

#### Task 4: Ticket Model & Service

**Files:**
- Create: `Assets/Scripts/Models/TicketModel.cs`
- Create: `Assets/Scripts/Services/TicketService.cs`

- [ ] **Step 1: Create TicketModel (pure data, no MonoBehaviour)**

```csharp
using System;

[System.Serializable]
public class TicketModel : IEquatable<TicketModel>
{
    public string Id { get; private set; }
    public TaskData TaskData { get; private set; }
    public bool IsStamped { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public TicketModel(TaskData taskData)
    {
        Id = Guid.NewGuid().ToString();
        TaskData = taskData;
        IsStamped = false;
        CreatedAt = DateTime.UtcNow;
    }

    public TicketModel WithStamped(bool stamped) => new()
    {
        Id = Id,
        TaskData = TaskData,
        IsStamped = stamped,
        CreatedAt = CreatedAt
    };

    public override bool Equals(object obj) => Equals(obj as TicketModel);
    public bool Equals(TicketModel other) => other != null && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
```

- [ ] **Step 2: Create TicketService**

```csharp
using System.Collections.Generic;
using UnityEngine;

public class TicketService
{
    private HashSet<TicketModel> _activeTickets = new();
    private int _maxActiveTickets;

    public TicketService(int maxActive = Constants.MAX_ACTIVE_TICKETS)
    {
        _maxActiveTickets = maxActive;
    }

    public TicketModel CreateTicket(TaskData taskData)
    {
        if (_activeTickets.Count >= _maxActiveTickets)
        {
            Debug.LogWarning($"TicketService: Max active tickets ({_maxActiveTickets}) reached");
            return null;
        }

        TicketModel ticket = new(taskData);
        _activeTickets.Add(ticket);
        return ticket;
    }

    public void StampTicket(TicketModel ticket)
    {
        if (_activeTickets.Contains(ticket))
        {
            TicketModel stamped = ticket.WithStamped(true);
            _activeTickets.Remove(ticket);
            _activeTickets.Add(stamped);

            GameEvents.Instance?.OnTicketStamped.Invoke(stamped);
        }
    }

    public void ShredTicket(TicketModel ticket)
    {
        if (_activeTickets.Contains(ticket))
        {
            _activeTickets.Remove(ticket);
            GameEvents.Instance?.OnTicketShredded.Invoke(ticket);
        }
    }

    public int GetActiveCount() => _activeTickets.Count;
    public bool HasRoom() => _activeTickets.Count < _maxActiveTickets;
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Models/TicketModel.cs Assets/Scripts/Services/TicketService.cs
git commit -m "feat: add ticket model and service"
```

---

#### Task 5: Minigame Interface & Factory

**Files:**
- Create: `Assets/Scripts/Minigames/IMinigame.cs`
- Create: `Assets/Scripts/Minigames/MinigameFactory.cs`

- [ ] **Step 1: Create IMinigame interface**

```csharp
using System;

public interface IMinigame
{
    MinigameType Type { get; }
    bool IsActive { get; }
    bool IsCompleted { get; }

    void Start(Action onComplete);
    void Close();
}
```

- [ ] **Step 2: Create MinigameFactory**

```csharp
using UnityEngine;

public class MinigameFactory
{
    private Transform _container;
    private IMinigame _currentMinigame;

    public MinigameFactory(Transform container)
    {
        _container = container;
    }

    public void OpenMinigame(MinigameType type, TypingTaskSO typingTask, System.Action onComplete)
    {
        if (_currentMinigame != null)
        {
            _currentMinigame.Close();
        }

        IMinigame minigame = CreateMinigameInstance(type, typingTask);
        if (minigame != null)
        {
            _currentMinigame = minigame;
            GameEvents.Instance?.OnMinigameStarted.Invoke(type);
            minigame.Start(() =>
            {
                GameEvents.Instance?.OnMinigameCompleted.Invoke(type);
                onComplete?.Invoke();
            });
        }
    }

    private IMinigame CreateMinigameInstance(MinigameType type, TypingTaskSO typingTask)
    {
        // Load prefab from Resources/Minigames/
        string prefabPath = $"Minigames/{type.ToString()}/{type.ToString()}Minigame";
        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"MinigameFactory: Prefab not found at {prefabPath}");
            return null;
        }

        GameObject instance = Object.Instantiate(prefab, _container);
        IMinigame minigame = instance.GetComponent<IMinigame>();

        // For typing minigames, pass the task data
        if (type == MinigameType.Typing && minigame is TypingMinigame typingMinigame)
        {
            typingMinigame.SetTask(typingTask);
        }

        return minigame;
    }

    public void Close()
    {
        if (_currentMinigame != null)
        {
            _currentMinigame.Close();
            _currentMinigame = null;
        }
    }

    public bool IsOpen => _currentMinigame != null && _currentMinigame.IsActive;
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/Minigames/IMinigame.cs Assets/Scripts/Minigames/MinigameFactory.cs
git commit -m "feat: add minigame interface and factory pattern"
```

---

### Phase 2: Core Services (Days 2-3)

#### Task 6: GameService (Main Orchestration)

**Files:**
- Create: `Assets/Scripts/Services/GameService.cs`

- [ ] **Step 1: Create GameService**

```csharp
using UnityEngine;

public class GameService : MonoBehaviour
{
    [SerializeField] private float dayDuration = Constants.DAY_DURATION;

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
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Services/GameService.cs
git commit -m "feat: add game service for main loop orchestration"
```

---

#### Task 7: DayService (Day Transitions & Upgrades)

**Files:**
- Create: `Assets/Scripts/Services/DayService.cs`

- [ ] **Step 1: Create DayService**

```csharp
using System.Collections;
using UnityEngine;
using TMPro;

public class DayService : MonoBehaviour
{
    [SerializeField] private GameObject upgradeModalPrefab;
    [SerializeField] private GameObject endingPanelPrefab;
    [SerializeField] private Canvas canvas;

    private GameObject _upgradeModalInstance;
    private GameObject _endingPanelInstance;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
    }

    public void ShowUpgradeModal(int completedDay)
    {
        if (_upgradeModalInstance == null)
        {
            _upgradeModalInstance = Instantiate(upgradeModalPrefab, canvas.transform);
        }

        _upgradeModalInstance.SetActive(true);
        UpgradeModalController modal = _upgradeModalInstance.GetComponent<UpgradeModalController>();
        if (modal != null)
        {
            modal.Setup(completedDay, () => _upgradeModalInstance.SetActive(false));
        }
    }

    public void ShowDayFailure(int day)
    {
        SystemLog.Instance?.LogMessage($"Day {day} failed - quota not met!");
    }

    public void ShowEnding()
    {
        Time.timeScale = 0f;

        if (_endingPanelInstance == null)
        {
            _endingPanelInstance = Instantiate(endingPanelPrefab, canvas.transform);
        }

        _endingPanelInstance.SetActive(true);
        EndingController ending = _endingPanelInstance.GetComponent<EndingController>();
        if (ending != null)
        {
            ending.PlayEnding();
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Services/DayService.cs
git commit -m "feat: add day service for transitions and upgrades"
```

---

#### Task 8: Bootstrapper (Dependency Injection)

**Files:**
- Create: `Assets/Scripts/Core/Bootstrapper.cs`

- [ ] **Step 1: Create Bootstrapper**

```csharp
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private Transform minigameContainer;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        Debug.Log("[Bootstrapper] Starting initialization...");

        // Setup event system first
        GameEvents events = GetComponentInChildren<GameEvents>();
        if (events == null)
        {
            GameObject eventsObj = new("GameEvents");
            eventsObj.transform.SetParent(transform);
            events = eventsObj.AddComponent<GameEvents>();
        }

        // Find or create canvas
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[Bootstrapper] No Canvas found in scene!");
                return;
            }
        }

        // Find or create minigame container
        if (minigameContainer == null)
        {
            Transform container = canvas.transform.Find("MinigameContainer");
            if (container != null)
            {
                minigameContainer = container;
            }
            else
            {
                GameObject containerObj = new("MinigameContainer");
                containerObj.transform.SetParent(canvas.transform);
                minigameContainer = containerObj.transform;
            }
        }

        // Initialize services
        GameState gameState = new();

        TypingTaskDatabase typingTaskDb = FindFirstObjectByType<TypingTaskDatabase>();
        if (typingTaskDb == null)
        {
            Debug.LogWarning("[Bootstrapper] TypingTaskDatabase not found. Creating one.");
            GameObject db = new("TypingTaskDatabase");
            typingTaskDb = db.AddComponent<TypingTaskDatabase>();
        }

        TaskService taskService = new(typingTaskDb);
        TicketService ticketService = new(Constants.MAX_ACTIVE_TICKETS);
        MinigameFactory minigameFactory = new(minigameContainer);

        // Get or create DayService
        DayService dayService = GetComponentInChildren<DayService>();
        if (dayService == null)
        {
            GameObject dayObj = new("DayService");
            dayObj.transform.SetParent(transform);
            dayService = dayObj.AddComponent<DayService>();
        }

        // Get or create GameService
        GameService gameService = GetComponentInChildren<GameService>();
        if (gameService == null)
        {
            GameObject gameObj = new("GameService");
            gameObj.transform.SetParent(transform);
            gameService = gameObj.AddComponent<GameService>();
        }

        // Initialize GameService with all dependencies
        gameService.Initialize(gameState, taskService, ticketService, minigameFactory, dayService);

        // Initialize Controllers
        InitializeControllers(canvas, gameState, taskService, ticketService, minigameFactory);

        Debug.Log("[Bootstrapper] ✓ All systems initialized");
    }

    private void InitializeControllers(Canvas canvas, GameState gameState, TaskService taskService,
                                      TicketService ticketService, MinigameFactory minigameFactory)
    {
        // Find or create GameHUDController
        GameHUDController hudController = canvas.GetComponentInChildren<GameHUDController>();
        if (hudController == null)
        {
            Transform hudPanel = canvas.transform.Find("GameHUD");
            if (hudPanel != null)
            {
                hudController = hudPanel.gameObject.AddComponent<GameHUDController>();
            }
        }

        if (hudController != null)
        {
            hudController.Initialize(gameState);
        }
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Core/Bootstrapper.cs
git commit -m "feat: add bootstrapper for dependency injection"
```

---

### Phase 3: UI Controllers & Views (Days 3-4)

#### Task 9: GameHUD Controller & View

**Files:**
- Create: `Assets/Scripts/UI/Controllers/GameHUDController.cs`
- Create: `Assets/Scripts/UI/Views/GameHUDView.cs`

- [ ] **Step 1: Create GameHUDView**

```csharp
using UnityEngine;
using TMPro;

public class GameHUDView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI tasksText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void SetDay(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {day}";
    }

    public void SetTasks(int completed, int required)
    {
        if (tasksText != null)
            tasksText.text = $"Tasks: {completed}/{required}";
    }

    public void SetTime(float hours)
    {
        if (timeText != null)
            timeText.text = $"Time: {FormatTime(hours)}";
    }

    public void SetTimer(float remaining)
    {
        if (timerText != null)
            timerText.text = $"Time left: {remaining:F1}s";
    }

    private string FormatTime(float hours)
    {
        int hour = Mathf.FloorToInt(hours);
        int minute = Mathf.FloorToInt((hours - hour) * 60f);
        string period = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;
        return $"{displayHour}:{minute:D2} {period}";
    }
}
```

- [ ] **Step 2: Create GameHUDController**

```csharp
using UnityEngine;

public class GameHUDController : MonoBehaviour
{
    private GameHUDView _view;
    private GameState _gameState;

    public void Initialize(GameState gameState)
    {
        _gameState = gameState;
        _view = GetComponent<GameHUDView>();
        if (_view == null)
        {
            _view = gameObject.AddComponent<GameHUDView>();
        }

        // Subscribe to events
        GameEvents.Instance.OnDayChanged.AddListener(OnDayChanged);
        GameEvents.Instance.OnTasksCompletedChanged.AddListener(OnTasksChanged);
        GameEvents.Instance.OnDayTimerUpdated.AddListener(OnTimerUpdated);

        // Initial state
        UpdateUI();
    }

    private void OnDayChanged(int day)
    {
        _view.SetDay(day);
    }

    private void OnTasksChanged(int count)
    {
        _view.SetTasks(count, Constants.TASKS_REQUIRED_PER_DAY);
    }

    private void OnTimerUpdated(float timer)
    {
        float timeProgress = timer / Constants.DAY_DURATION;
        float displayTime = Constants.START_TIME + (Constants.END_TIME - Constants.START_TIME) * timeProgress;
        _view.SetTime(displayTime);

        float remaining = Constants.DAY_DURATION - timer;
        _view.SetTimer(remaining);
    }

    private void UpdateUI()
    {
        _view.SetDay(_gameState.Current.CurrentDay);
        _view.SetTasks(_gameState.Current.TasksCompleted, Constants.TASKS_REQUIRED_PER_DAY);
        OnTimerUpdated(_gameState.Current.DayTimer);
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/UI/Controllers/GameHUDController.cs Assets/Scripts/UI/Views/GameHUDView.cs
git commit -m "feat: add game HUD controller and view"
```

---

#### Task 10: TicketView & Factory

**Files:**
- Create: `Assets/Scripts/UI/Views/TicketView.cs`
- Create: `Assets/Scripts/UI/Utilities/TicketViewFactory.cs`

- [ ] **Step 1: Create TicketView**

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class TicketView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button startTaskButton;
    [SerializeField] private Image stampImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private TicketModel _model;
    private RectTransform _rect;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isExpanded = false;
    private Coroutine _expandCoroutine;

    public TicketModel Model => _model;

    public void SetupWithModel(TicketModel model, System.Action onStartTask)
    {
        _model = model;
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>().rootCanvas;

        // Setup UI
        if (titleText != null)
            titleText.text = model.TaskData.Title;
        if (descriptionText != null)
            descriptionText.text = model.TaskData.Description;

        // Setup button
        if (startTaskButton != null)
        {
            startTaskButton.gameObject.SetActive(model.TaskData.RequiresMinigame);
            startTaskButton.onClick.AddListener(() => onStartTask?.Invoke());
        }

        if (stampImage != null)
            stampImage.gameObject.SetActive(false);

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void UpdateModel(TicketModel newModel)
    {
        _model = newModel;

        if (newModel.IsStamped)
        {
            ShowStamp();
            if (startTaskButton != null)
                startTaskButton.gameObject.SetActive(false);
        }
    }

    public void ShowStamp()
    {
        if (stampImage != null)
            stampImage.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_model.IsStamped)
            ToggleExpand();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalPosition = _rect.anchoredPosition;
        _originalParent = transform.parent;

        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectTransform(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.worldCamera,
            out Vector2 localPoint);
        _rect.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check for shredder drop
        List<RaycastResult> results = new();
        GraphicRaycaster raycaster = _canvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                ShredderDropZone shredder = result.gameObject.GetComponent<ShredderDropZone>();
                if (shredder != null && _model.IsStamped)
                {
                    shredder.OnTicketDropped(_model);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // No valid drop - return to original position
        transform.SetParent(_originalParent);
        _rect.anchoredPosition = _originalPosition;
    }

    private void ToggleExpand()
    {
        if (_expandCoroutine != null)
            StopCoroutine(_expandCoroutine);

        _expandCoroutine = StartCoroutine(ExpandCoroutine(!_isExpanded));
    }

    private IEnumerator ExpandCoroutine(bool shouldExpand)
    {
        if (shouldExpand && _canvas != null)
        {
            transform.SetParent(_canvas.transform);
            transform.SetAsLastSibling();
        }

        Vector3 startScale = _rect.localScale;
        Vector3 endScale = shouldExpand ? Vector3.one : new Vector3(Constants.TICKET_MINI_SCALE, Constants.TICKET_MINI_SCALE, 1f);

        float elapsed = 0f;
        while (elapsed < Constants.TICKET_EXPAND_DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / Constants.TICKET_EXPAND_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            _rect.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        _rect.localScale = endScale;
        _isExpanded = shouldExpand;
    }
}
```

- [ ] **Step 2: Create TicketViewFactory**

```csharp
using UnityEngine;

public class TicketViewFactory
{
    private GameObject _ticketPrefab;
    private Transform _container;

    public TicketViewFactory(GameObject prefab, Transform container)
    {
        _ticketPrefab = prefab;
        _container = container;
    }

    public TicketView CreateTicketView(TicketModel model, System.Action onStartTask)
    {
        if (_ticketPrefab == null)
        {
            Debug.LogError("TicketViewFactory: Prefab is null");
            return null;
        }

        GameObject instance = Object.Instantiate(_ticketPrefab, _container);
        instance.name = $"Ticket_{model.Id}";

        TicketView view = instance.GetComponent<TicketView>();
        if (view == null)
            view = instance.AddComponent<TicketView>();

        view.SetupWithModel(model, onStartTask);
        return view;
    }
}
```

- [ ] **Step 3: Commit**

```bash
git add Assets/Scripts/UI/Views/TicketView.cs Assets/Scripts/UI/Utilities/TicketViewFactory.cs
git commit -m "feat: add ticket view and factory"
```

---

### Phase 4: Minigame Base & Typing Implementation (Days 4-5)

#### Task 11: BaseMinigame & TypingMinigame

**Files:**
- Create: `Assets/Scripts/Minigames/Base/BaseMinigame.cs`
- Create: `Assets/Scripts/Minigames/Typing/TypingMinigame.cs`
- Create: `Assets/Scripts/Minigames/Typing/TypingMinigameView.cs`

- [ ] **Step 1: Create BaseMinigame**

```csharp
using UnityEngine;
using System.Collections;

public abstract class BaseMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] protected GameObject minigameWindow;
    [SerializeField] protected float completionDelay = 2f;

    public abstract MinigameType Type { get; }
    public bool IsActive { get; protected set; }
    public bool IsCompleted { get; protected set; }

    protected System.Action _onComplete;

    public virtual void Start(System.Action onComplete)
    {
        _onComplete = onComplete;
        IsActive = true;
        IsCompleted = false;

        if (minigameWindow != null)
            minigameWindow.SetActive(true);
    }

    public virtual void Close()
    {
        if (minigameWindow != null)
            minigameWindow.SetActive(false);

        IsActive = false;
    }

    protected virtual void CompleteMinigame()
    {
        if (IsCompleted) return;

        IsCompleted = true;
        IsActive = false;
        _onComplete?.Invoke();

        StartCoroutine(CloseAfterDelay(completionDelay));
    }

    protected IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Close();
    }
}
```

- [ ] **Step 2: Create TypingMinigameView**

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TypingMinigameView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image windowBackground;

    public void SetPrompt(string text)
    {
        if (promptText != null)
            promptText.text = text;
    }

    public void SetInputPlaceholder(string placeholder)
    {
        if (inputField != null)
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeholder;
    }

    public void SetupSubmitButton(System.Action callback)
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(() => callback?.Invoke());
    }

    public string GetInputText()
    {
        return inputField != null ? inputField.text : "";
    }

    public void ClearInput()
    {
        if (inputField != null)
            inputField.text = "";
    }

    public void ShowFeedback(string message, bool isCorrect)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }

    public void FocusInput()
    {
        if (inputField != null)
            inputField.Select();
    }
}
```

- [ ] **Step 3: Create TypingMinigame**

```csharp
using UnityEngine;
using System.Collections;

public class TypingMinigame : BaseMinigame
{
    public override MinigameType Type => MinigameType.Typing;

    [SerializeField] private TypingMinigameView view;
    private TypingTaskSO _currentTask;

    public void SetTask(TypingTaskSO task)
    {
        _currentTask = task;
    }

    public override void Start(System.Action onComplete)
    {
        base.Start(onComplete);

        if (_currentTask == null)
        {
            Debug.LogError("TypingMinigame: No task set!");
            CompleteMinigame();
            return;
        }

        if (view == null)
            view = GetComponentInChildren<TypingMinigameView>();

        view.SetPrompt(_currentTask.prompt);
        view.SetInputPlaceholder("Type your response...");
        view.SetupSubmitButton(OnSubmit);
        view.FocusInput();
    }

    private void OnSubmit()
    {
        string input = view.GetInputText().Trim();

        if (string.IsNullOrEmpty(input))
        {
            view.ShowFeedback("Please type something!", false);
            return;
        }

        // Simple validation - check if input is not empty
        // In a real game, you'd have more sophisticated checks
        view.ShowFeedback("✓ Response submitted!", true);
        view.ClearInput();

        StartCoroutine(CompleteAfterFeedback());
    }

    private IEnumerator CompleteAfterFeedback()
    {
        yield return new WaitForSeconds(1f);
        CompleteMinigame();
    }
}
```

- [ ] **Step 4: Commit**

```bash
git add Assets/Scripts/Minigames/Base/BaseMinigame.cs Assets/Scripts/Minigames/Typing/TypingMinigame.cs Assets/Scripts/Minigames/Typing/TypingMinigameView.cs
git commit -m "feat: add base minigame and typing minigame implementation"
```

---

#### Task 12: ShredderDropZone & Input Handler

**Files:**
- Create: `Assets/Scripts/UI/Utilities/ShredderDropZone.cs`
- Modify: `Assets/Scripts/UI/Views/TicketView.cs` (connect to shredder)

- [ ] **Step 1: Create ShredderDropZone**

```csharp
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShredderDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Image image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            image.raycastTarget = true;
        }
    }

    public void OnTicketDropped(TicketModel ticket)
    {
        Debug.Log($"Shredded ticket: {ticket.TaskData.Title}");

        // Notify services that ticket was shredded
        GameEvents.Instance?.OnTicketShredded.Invoke(ticket);

        // Play shred animation
        StartCoroutine(ShredAnimation());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = activeAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = inactiveAlpha;
    }

    private System.Collections.IEnumerator ShredAnimation()
    {
        // Could add particle effects, rotation, sound here
        Debug.Log("Shredding animation...");
        yield return new System.Collections.Generic.WaitForSeconds(0.5f);
    }
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/UI/Utilities/ShredderDropZone.cs
git commit -m "feat: add shredder drop zone"
```

---

#### Task 13: Printer System Refactor

**Files:**
- Create: `Assets/Scripts/UI/Controllers/PrinterController.cs`

- [ ] **Step 1: Create PrinterController**

```csharp
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
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/UI/Controllers/PrinterController.cs
git commit -m "feat: add printer controller"
```

---

### Phase 5: Scene Setup & Final Integration (Days 5-6)

#### Task 14: Update Bootstrapper to Wire Everything

**Files:**
- Modify: `Assets/Scripts/Core/Bootstrapper.cs`

- [ ] **Step 1: Update Bootstrapper with full initialization**

```csharp
// Add to Bootstrapper.cs InitializeControllers method:

private void InitializeControllers(Canvas canvas, GameState gameState, TaskService taskService,
                                  TicketService ticketService, MinigameFactory minigameFactory)
{
    // GameHUD
    GameHUDController hudController = canvas.GetComponentInChildren<GameHUDController>();
    if (hudController == null)
    {
        Transform hudPanel = canvas.transform.Find("GameHUD");
        if (hudPanel != null)
        {
            hudController = hudPanel.gameObject.AddComponent<GameHUDController>();
        }
    }
    if (hudController != null)
        hudController.Initialize(gameState);

    // Printer
    PrinterController printerController = canvas.GetComponentInChildren<PrinterController>();
    if (printerController == null)
    {
        Transform printerUI = canvas.transform.Find("Printer");
        if (printerUI != null)
        {
            printerController = printerUI.gameObject.AddComponent<PrinterController>();
        }
    }

    GameService gameService = GetComponentInChildren<GameService>();
    if (printerController != null && gameService != null)
        printerController.Initialize(gameService, minigameFactory);
}
```

- [ ] **Step 2: Commit**

```bash
git add Assets/Scripts/Core/Bootstrapper.cs
git commit -m "feat: complete bootstrapper wiring"
```

---

#### Task 15: Create Scene Setup Guide

**Files:**
- Create: `docs/SCENE_SETUP.md`

- [ ] **Step 1: Create comprehensive scene setup guide**

```markdown
# Scene Setup Guide — While You Were Busy

## Canvas Structure

```
Canvas (ScreenSpaceOverlay, GraphicRaycaster)
├── Background (Image, Wallpaper sprite, raycastTarget=false, sortingOrder=0)
├── Desk (Image, Desk sprite, raycastTarget=false, sortingOrder=1)
├── GameHUD (Panel, sortingOrder=10)
│   ├── DayText (TextMeshProUGUI)
│   ├── TasksText (TextMeshProUGUI)
│   ├── TimeText (TextMeshProUGUI)
│   └── TimerText (TextMeshProUGUI)
├── TicketBoard (Panel, sortingOrder=2)
│   └── [TicketView instances instantiated at runtime]
├── Printer (Image + Panel, sortingOrder=5)
│   ├── PrintButton (Button)
│   ├── StatusText (TextMeshProUGUI)
│   └── PrinterAnimation (Image)
├── Shredder (Image + ShredderDropZone, sortingOrder=5)
│   └── [Drop target for stamped tickets]
├── MinigameContainer (Panel, sortingOrder=20)
│   └── [Minigame instances loaded at runtime]
├── ModalContainer (Panel, sortingOrder=30)
│   └── [Modals instantiated at runtime]
└── Overlays (Panel, sortingOrder=40)
    └── [Tooltips, effects, etc]
```

## Prefabs to Create

### TicketView Prefab
- Location: `Assets/Resources/UI/TicketView.prefab`
- Components:
  - RectTransform (sizeDelta: 140x77)
  - Image (sprite: ticket sprite)
  - TicketView.cs script
  - CanvasGroup
  - EventTrigger (drag handlers)
  - Children:
    - TitleText (TextMeshProUGUI)
    - DescriptionText (TextMeshProUGUI)
    - StampImage (Image, inactive by default)
    - StartTaskButton (Button, inactive by default)

### Minigame Prefabs
- Location: `Assets/Resources/Minigames/{Type}/`

**TypingMinigame.prefab:**
- Components:
  - RectTransform (full screen overlay)
  - Image (semi-transparent background)
  - TypingMinigame.cs
  - Children:
    - Window (Panel)
      - PromptText (TextMeshProUGUI)
      - InputField (TMP_InputField)
      - SubmitButton (Button)
      - FeedbackText (TextMeshProUGUI)

## Initialization Order

1. Bootstrapper.Awake() creates all services
2. GameService.Start() calls StartNewDay()
3. PrinterController.Initialize() wires button
4. Printer prints first ticket immediately (printInterval = 15s)
5. Player clicks Start Task button on ticket
6. MinigameFactory.OpenMinigame() loads prefab
7. Minigame completes, calls callback
8. TicketModel stamped, view shows stamp
9. Player drags to shredder
10. ShredderDropZone calls GameEvents.OnTicketShredded
11. GameService updates state
12. Day ends, win/lose check

## Configuration

All magic numbers are in `Constants.cs`. Modify there for balance.
```

- [ ] **Step 2: Commit**

```bash
git add docs/SCENE_SETUP.md
git commit -m "docs: add comprehensive scene setup guide"
```

---

#### Task 16: Delete Old Broken Code

**Files:**
- Delete: `Assets/Scripts/.archive_setup_scripts/`
- Delete: `Assets/Scripts/Core/GameManager.cs` (replaced by GameService)
- Delete: `Assets/Scripts/Core/MinigameManager.cs` (replaced by MinigameFactory)
- Delete: `Assets/Scripts/Core/Printer.cs` (replaced by PrinterController)
- Delete: `Assets/Scripts/Core/Ticket.cs` (replaced by TicketModel/TicketView)
- Delete: `Assets/Scripts/Core/BulletinBoard.cs` (no longer needed)
- Delete: `Assets/Scripts/Core/ShredderUI.cs` (replaced by ShredderDropZone)
- Delete: `Assets/Scripts/Data/TaskDatabase.cs` (replaced by TaskService)
- Delete: `Assets/Scripts/UI/DayManager.cs` (replaced by DayService)
- Delete: `Assets/Scripts/UI/UpgradeManager.cs`
- Delete: Old minigame implementations

- [ ] **Step 1: Remove archive scripts directory**

```bash
git rm -r Assets/Scripts/.archive_setup_scripts/
```

- [ ] **Step 2: Remove old core scripts**

```bash
git rm Assets/Scripts/Core/GameManager.cs \
         Assets/Scripts/Core/MinigameManager.cs \
         Assets/Scripts/Core/Printer.cs \
         Assets/Scripts/Core/Ticket.cs \
         Assets/Scripts/Core/BulletinBoard.cs \
         Assets/Scripts/Core/ShredderUI.cs
```

- [ ] **Step 3: Remove old data scripts**

```bash
git rm Assets/Scripts/Data/TaskDatabase.cs
```

- [ ] **Step 4: Remove old UI scripts**

```bash
git rm Assets/Scripts/UI/DayManager.cs \
         Assets/Scripts/UI/UpgradeManager.cs \
         Assets/Scripts/UI/UISetup.cs \
         Assets/Scripts/UI/AutoSetupScene.cs
```

- [ ] **Step 5: Commit**

```bash
git commit -m "refactor: remove old broken architecture"
```

---

#### Task 17: Remaining Minigames (Math, MultipleChoice, PhotoReveal)

**Files:**
- Create: `Assets/Scripts/Minigames/Math/MathMinigame.cs`
- Create: `Assets/Scripts/Minigames/Math/MathMinigameView.cs`
- Create: `Assets/Scripts/Minigames/MultipleChoice/MultipleChoiceMinigame.cs`
- Create: `Assets/Scripts/Minigames/MultipleChoice/MultipleChoiceMinigameView.cs`
- Create: `Assets/Scripts/Minigames/PhotoReveal/PhotoRevealMinigame.cs`
- Create: `Assets/Scripts/Minigames/PhotoReveal/PhotoRevealMinigameView.cs`

- [ ] **Step 1-6: Implement each minigame following the same pattern as TypingMinigame**

(Each minigame follows identical pattern — implement all 6 at once with batch_execute or in parallel tasks)

- [ ] **Step 7: Commit all minigames**

```bash
git add Assets/Scripts/Minigames/
git commit -m "feat: implement math, multiple choice, and photo reveal minigames"
```

---

#### Task 18: End-to-End Testing & Polish

**Files:**
- Create: `Assets/Scripts/Editor/MinigameSceneValidator.cs`
- Modify: All views (add null checks, error handling)

- [ ] **Step 1: Create scene validator**

```csharp
using UnityEngine;
using UnityEditor;

public class MinigameSceneValidator
{
    [MenuItem("Tools/Validate Game Scene")]
    public static void ValidateScene()
    {
        Debug.Log("=== Scene Validation ===");

        // Check Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found in scene!");
            return;
        }
        Debug.Log("✓ Canvas found");

        // Check Bootstrapper
        Bootstrapper bootstrapper = FindFirstObjectByType<Bootstrapper>();
        if (bootstrapper == null)
        {
            Debug.LogError("❌ No Bootstrapper found in scene!");
            return;
        }
        Debug.Log("✓ Bootstrapper found");

        // Check required child panels
        string[] requiredPanels = { "GameHUD", "TicketBoard", "Printer", "Shredder", "MinigameContainer", "ModalContainer" };
        foreach (string panelName in requiredPanels)
        {
            Transform panel = canvas.transform.Find(panelName);
            if (panel == null)
                Debug.LogWarning($"⚠ Missing panel: {panelName}");
            else
                Debug.Log($"✓ Found {panelName}");
        }

        // Check minigame prefabs
        string[] minigameTypes = { "Typing", "Math", "MultipleChoice", "PhotoReveal" };
        foreach (string type in minigameTypes)
        {
            GameObject prefab = Resources.Load<GameObject>($"Minigames/{type}/{type}Minigame");
            if (prefab == null)
                Debug.LogWarning($"⚠ Missing minigame prefab: {type}");
            else
                Debug.Log($"✓ Found {type} minigame prefab");
        }

        Debug.Log("=== Validation Complete ===");
    }
}
```

- [ ] **Step 2: Run validator and fix issues**

```
Tools/Validate Game Scene
```

- [ ] **Step 3: Test full game loop**

- Play game
- Verify Day 1 starts with timer
- Click Print button, verify ticket spawns
- Click ticket to expand
- Click Start Task
- Verify typing minigame opens
- Type and submit
- Verify ticket shows stamp
- Drag to shredder
- Verify ticket disappears
- Repeat 4 times to complete day quota
- Verify Sleep button becomes active
- Click Sleep, verify upgrade modal
- Proceed to Day 2

- [ ] **Step 4: Commit**

```bash
git add Assets/Scripts/Editor/MinigameSceneValidator.cs
git commit -m "feat: add scene validator and polish"
```

---

## Summary

**Total commits:** 18 major feature commits
**Total files created:** 35+
**Total old code removed:** 10+ files
**New architecture:** Clean, testable, scalable to 10+ minigames

**Key improvements over old code:**
- ✅ Zero singletons (replaced with DI via Bootstrapper)
- ✅ Clear separation of concerns (Models, Services, Views, Controllers)
- ✅ IMinigame interface makes adding new minigames trivial (1 script + 1 prefab)
- ✅ GameEvents pub-sub replaces hidden dependencies
- ✅ Ticket is now pure data (TicketModel) + view (TicketView) — no drag logic in data
- ✅ All UI setup at edit-time (no AutoSetupScene runtime creation)
- ✅ Single Bootstrapper entry point — predictable initialization
- ✅ Constants.cs centralizes all balance values
- ✅ Testable (no static refs, proper DI)

---

## Execution

Choose execution approach:

1. **Subagent-Driven** (recommended) — Fresh agent per task, review between tasks
2. **Inline Execution** — Execute all tasks in this session with checkpoints
