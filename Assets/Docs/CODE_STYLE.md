# Code Style Guide

## Naming Conventions

### Classes
- Use PascalCase
- Examples: `GameManager`, `TypingMinigameUI`, `BulletinBoard`
- Suffixes: `Manager`, `UI`, `Controller`, `Service`

### Methods
- Use PascalCase
- Public: `StartMinigame()`, `CompleteTask()`, `OnButtonClicked()`
- Private: `GenerateProblem()`, `UpdateProgress()`
- Callbacks: `OnMinigameCompleted()`, `OnTaskStarted()`

### Fields
- Private: camelCase
  - Example: `private int currentDay;`
- Serialized: `[SerializeField] private int currentDay;`
- Public: camelCase (rare, prefer properties)
  - Example: `public int CurrentDay { get; private set; }`

### Constants
- UPPER_CASE with underscores
- Example: `private const float SPAWN_INTERVAL = 15f;`

### Boolean Fields
- Prefix with `is`, `has`, `can`
- Examples: `isActive`, `hasSpawned`, `canStart`

---

## Class Structure

Order of members:
1. Fields (public, then private)
2. Properties (public, then private)
3. Events (if any)
4. Lifecycle methods (Awake, Start, OnEnable, OnDisable, OnDestroy)
5. Public methods
6. Private methods
7. Coroutines

---

## Comments & Documentation

### XMLDoc Comments (for public APIs)
```csharp
/// <summary>
/// Starts the minigame with the given task and completion callback.
/// </summary>
/// <param name="task">The task data for this minigame</param>
/// <param name="onComplete">Called when minigame is finished</param>
public void StartMinigame(TypingTaskSO task, System.Action onComplete)
{
}
```

### Inline Comments (only for complex logic)
```csharp
// Use ternary only for simple conditions
bool isReady = hasInitialized && currentDay > 0;

// Multi-line complex logic gets a comment
// Fisher-Yates shuffle to randomize tiles
for (int i = tiles.Length - 1; i > 0; i--)
{
    int randomIndex = Random.Range(0, i + 1);
    // Swap
    Tile temp = tiles[i];
    tiles[i] = tiles[randomIndex];
    tiles[randomIndex] = temp;
}
```

### DO NOT Comment
- Obvious code: `int count = 0; // Set count to zero` ✗
- Method bodies that mirror the method name
- Commented-out code (delete it instead)

---

## Logging

### Use Structured Logging
```csharp
// Good ✓
Debug.Log("[GameBootstrapper] ✓ GameManager ready");
Debug.LogError("[Printer] Cannot find spawn point!");
Debug.LogWarning("[MinigameManager] Minigame null, using fallback");

// Bad ✗
Debug.Log("GameManager ready");
Debug.Log("Error error error!");
```

### Levels
- `Debug.Log()` — Major initialization/flow events only
- `Debug.LogWarning()` — Recoverable issues, fallbacks triggered
- `Debug.LogError()` — Failures that break functionality

### No Debug Spam
- Avoid logging in `Update()` every frame
- Avoid logging every ticket spawn (do it once per 5 spawns or log summary)
- Only log enough to trace a bug

---

## Serialization & Inspector

### SerializeFields
```csharp
[Header("Gameplay Settings")]
[SerializeField] private float spawnInterval = 15f;
[SerializeField] private int requiredTasksPerDay = 4;

[Header("References")]
[SerializeField] private Canvas gameCanvas;
[SerializeField] private Transform spawnPoint;

[Header("UI Prefabs")]
[SerializeField] private TypingMinigameUI typingMinigamePrefab;
```

### Inspector Visibility
- Organize with `[Header("Category")]`
- Use `[Tooltip("Description")]` for complex fields
- Mark unused fields as `[HideInInspector]`
- Private fields stay private (no need to expose)

---

## Scripting Patterns

### Singleton Pattern (for managers)
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
```

### Event Callbacks
```csharp
// Good: Clear callback pattern
public void StartMinigame(System.Action onComplete)
{
    completionCallback = onComplete;
    // ... setup
}

private void OnTaskDone()
{
    completionCallback?.Invoke();
}

// Avoid: Direct method calls unless necessary
```

### Coroutines
```csharp
// Good: Clear name, obvious what it does
private IEnumerator CloseAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    CloseMinigame();
}

// Start it:
StartCoroutine(CloseAfterDelay(2f));
```

---

## Error Handling

### DO
- Check for null before using: `if (gameObject != null)`
- Log errors with context: `Debug.LogError("[System] Missing ref: " + fieldName)`
- Provide fallbacks when possible

### DON'T
- Silent failures (always log)
- Generic error messages
- Try/catch for expected None (use null checks instead)

---

## Performance

### Best Practices
- Cache `GetComponent<>()` results in `Awake`
- Use object pools for frequently instantiated objects (Tickets, Tiles)
- Avoid `FindObjectsByType` in tight loops
- Use `OnEnable`/`OnDisable` for listeners (cleaner than manual cleanup)

### Avoid
- Allocating arrays/lists in `Update()`
- String concatenation in loops (use StringBuilder if needed)
- Excessive coroutines (use timers instead for simple cases)

---

## Testing

### Manual Testing Checklist
- [ ] Scene loads without console errors
- [ ] All UI elements appear in correct positions
- [ ] Buttons are clickable
- [ ] Minigames open/close correctly
- [ ] No memory leaks (check Profiler)
- [ ] Touch/mouse input works as expected

### Unit Test Structure (if using Unity Test Framework)
```csharp
[Test]
public void GameManager_CompleteTask_IncrementsCounter()
{
    var gameManager = new GameObject().AddComponent<GameManager>();
    gameManager.StartDay(1);
    gameManager.CompleteTask();
    Assert.AreEqual(1, gameManager.GetTaskCount());
}
```
