using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all available tasks that can be assigned to tickets.
/// Tasks scale in difficulty and reward as the game progresses.
/// </summary>
[System.Serializable]
public class TaskData
{
    public string title;
    public string description;
    public float baseReward;
    public int difficultyLevel;

    [Header("Minigame Settings")]
    [Tooltip("If set, this task requires a minigame to complete")]
    public bool requiresMinigame = false;

    [Tooltip("The typing task data (if this is a typing minigame task)")]
    public TypingTaskSO typingTask = null;

    public TaskData(string title, string description, float baseReward, int difficulty)
    {
        this.title = title;
        this.description = description;
        this.baseReward = baseReward;
        this.difficultyLevel = difficulty;
        this.requiresMinigame = false;
        this.typingTask = null;
    }

    /// <summary>
    /// Constructor for minigame tasks
    /// </summary>
    public TaskData(string title, string description, float baseReward, int difficulty, TypingTaskSO typingTaskData)
    {
        this.title = title;
        this.description = description;
        this.baseReward = baseReward;
        this.difficultyLevel = difficulty;
        this.requiresMinigame = typingTaskData != null;
        this.typingTask = typingTaskData;
    }
}

public class TaskDatabase : MonoBehaviour
{
    public static TaskDatabase Instance { get; private set; }

    [Header("Task Collections")]
    [SerializeField] private List<TaskData> easyTasks = new List<TaskData>();
    [SerializeField] private List<TaskData> mediumTasks = new List<TaskData>();
    [SerializeField] private List<TaskData> hardTasks = new List<TaskData>();

    private bool isInitialized = false;

    private void Awake()
    {
        // Singleton pattern
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
        EnsureInitialized();
    }

    /// <summary>
    /// Lazily initializes task pools. Safe to call from anywhere — guards against
    /// script execution order races where Printer calls GetRandomTask before Start fires.
    /// </summary>
    private void EnsureInitialized()
    {
        if (isInitialized) return;

        // Ensure TypingTaskDatabase exists before initializing tasks.
        // If it's not in the scene, create one now so typing tasks can be assigned.
        if (TypingTaskDatabase.Instance == null)
        {
            Debug.LogWarning("TaskDatabase: TypingTaskDatabase not found in scene. Creating one automatically.");
            new GameObject("TypingTaskDatabase").AddComponent<TypingTaskDatabase>();
        }

        InitializeDefaultTasks();
        isInitialized = true;
    }

    /// <summary>
    /// Initialize default task templates
    /// </summary>
    private void InitializeDefaultTasks()
    {
        // Clear lists first to remove any stale/empty serialized entries
        easyTasks.Clear();
        mediumTasks.Clear();
        hardTasks.Clear();

        // Easy tasks (Days 1+)
        AddTypingTask(easyTasks, "Draft Status Email", "Compose a brief status update", 15f, 1);
        AddTypingTask(easyTasks, "Quick System Update", "Send quick notification", 12f, 1);
        AddTypingTask(easyTasks, "Meeting Confirmation", "Confirm meeting attendance", 10f, 1);
        AddTypingTask(easyTasks, "Daily Log Entry", "Record daily activities", 12f, 1);
        AddTypingTask(easyTasks, "Backup Confirmation", "Confirm backup completion", 10f, 1);

        // Medium tasks (Days 2+)
        AddTypingTask(mediumTasks, "Security Protocol Update", "Document new security measures", 65f, 2);
        AddTypingTask(mediumTasks, "Meeting Minutes", "Record system status meeting", 60f, 2);
        AddTypingTask(mediumTasks, "Performance Report", "Draft quarterly performance analysis", 70f, 2);
        AddTypingTask(mediumTasks, "Stakeholder Email", "Compose update for stakeholders", 55f, 2);
        AddTypingTask(mediumTasks, "Technical Documentation", "Document system changes", 65f, 2);

        // Data Entry tasks (Days 2+)
        AddDataEntryTask(mediumTasks, "Match Sequence 4-digit", "Reproduce 4-digit number", 50f, 2);
        AddDataEntryTask(mediumTasks, "Match Sequence 5-digit", "Reproduce 5-digit number", 60f, 2);

        // Hard tasks (Days 3+)
        AddTypingTask(hardTasks, "Incident Report", "File detailed incident analysis", 250f, 3);
        AddTypingTask(hardTasks, "System Architecture Document", "Draft comprehensive technical documentation", 280f, 3);
        AddTypingTask(hardTasks, "Executive Summary", "Prepare executive briefing", 300f, 3);
        AddTypingTask(hardTasks, "Audit Response", "Respond to compliance audit", 270f, 3);
        AddTypingTask(hardTasks, "Crisis Communication", "Draft urgent stakeholder communication", 290f, 3);

        // File Sorting tasks (Days 3+)
        AddFileSortingTask(hardTasks, "Sort Records by Date", "Arrange records chronologically", 180f, 3);
        AddFileSortingTask(hardTasks, "Organize Archives", "Sort historical documents", 200f, 3);

        // Form Review tasks (Days 4+)
        AddFormReviewTask(hardTasks, "Verify Expense Form", "Find the error in expense report", 150f, 3);
        AddFormReviewTask(hardTasks, "Audit Employee Record", "Spot the mistake in personnel form", 170f, 3);
    }

    /// <summary>
    /// Add a data entry task
    /// </summary>
    private void AddDataEntryTask(List<TaskData> taskPool, string title, string description, float reward, int difficulty)
    {
        TaskData taskData = new TaskData(title, description, reward, difficulty);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Add a file sorting task
    /// </summary>
    private void AddFileSortingTask(List<TaskData> taskPool, string title, string description, float reward, int difficulty)
    {
        TaskData taskData = new TaskData(title, description, reward, difficulty);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Add a form review task
    /// </summary>
    private void AddFormReviewTask(List<TaskData> taskPool, string title, string description, float reward, int difficulty)
    {
        TaskData taskData = new TaskData(title, description, reward, difficulty);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Helper to add a typing task to a task pool
    /// </summary>
    private void AddTypingTask(List<TaskData> taskPool, string title, string description, float reward, int difficulty)
    {
        // Get a random typing task from the TypingTaskDatabase
        TypingTaskSO typingTask = null;

        if (TypingTaskDatabase.Instance != null)
        {
            typingTask = TypingTaskDatabase.Instance.GetRandomTypingTask();
            if (typingTask == null)
            {
                Debug.LogWarning($"TaskDatabase: TypingTaskDatabase returned null for task '{title}'. Is 'Create Sample Tasks At Runtime' checked?");
            }
        }
        else
        {
            Debug.LogWarning($"TaskDatabase: TypingTaskDatabase.Instance is null when creating task '{title}'. Make sure TypingTaskDatabase exists in scene.");
        }

        // Create task data with typing minigame reference
        TaskData taskData = new TaskData(title, description, reward, difficulty, typingTask);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Get a random task based on difficulty level
    /// </summary>
    public TaskData GetRandomTask(int difficultyLevel = 1)
    {
        EnsureInitialized();
        List<TaskData> taskPool = null;

        switch (difficultyLevel)
        {
            case 1:
                taskPool = easyTasks;
                break;
            case 2:
                taskPool = mediumTasks;
                break;
            case 3:
                taskPool = hardTasks;
                break;
            default:
                taskPool = easyTasks;
                break;
        }

        if (taskPool.Count > 0)
        {
            int randomIndex = Random.Range(0, taskPool.Count);
            return taskPool[randomIndex];
        }

        // Fallback task if pool is empty
        return new TaskData("Generic Task", "Process outputs efficiently", 10f, 1);
    }

    /// <summary>
    /// Get a random task based on current game day (gates minigame types)
    /// </summary>
    public TaskData GetRandomTaskForDay(int currentDay, int difficultyLevel)
    {
        EnsureInitialized();

        // Day 1: typing only
        // Days 2+: typing + data entry
        // Days 3+: typing + data entry + file sorting
        // Days 4+: all types (+ form review)

        // For now, return a random task from the appropriate difficulty pool
        // The task type gating will be handled in UI layer
        return GetRandomTask(difficultyLevel);
    }

    /// <summary>
    /// Get task difficulty based on current game progression
    /// </summary>
    public int GetDifficultyForCurrentProgress()
    {
        if (GameManager.Instance == null) return 1;

        float outputs = GameManager.Instance.GetOutputs();

        // Scale difficulty with player progression
        if (outputs < 100f) return 1;      // Easy tasks
        if (outputs < 500f) return 2;      // Medium tasks
        return 3;                          // Hard tasks
    }

    /// <summary>
    /// Add custom task to database
    /// </summary>
    public void AddCustomTask(string title, string description, float reward, int difficulty)
    {
        TaskData newTask = new TaskData(title, description, reward, difficulty);

        switch (difficulty)
        {
            case 1:
                easyTasks.Add(newTask);
                break;
            case 2:
                mediumTasks.Add(newTask);
                break;
            case 3:
                hardTasks.Add(newTask);
                break;
        }
    }
}
