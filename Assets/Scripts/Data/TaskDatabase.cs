using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Minigame type enum — controls which UI is launched
/// </summary>
public enum MinigameType { None, Typing, MultipleChoice, Math, PhotoReveal }

/// <summary>
/// Stores all available tasks that can be assigned to tickets.
/// Progression is driven by stacking minigames + upgrade rewards.
/// </summary>
[System.Serializable]
public class TaskData
{
    public string title;
    public string description;
    public float baseReward;

    [Header("Minigame Settings")]
    public MinigameType minigameType = MinigameType.None;
    public bool requiresMinigame => minigameType != MinigameType.None;

    [Tooltip("The typing task data (if this is a typing minigame task)")]
    public TypingTaskSO typingTask = null;

    public TaskData(string title, string description, float baseReward, MinigameType type)
    {
        this.title = title;
        this.description = description;
        this.baseReward = baseReward;
        this.minigameType = type;
        this.typingTask = null;
    }

    /// <summary>
    /// Constructor for typing tasks (minigame type is inferred)
    /// </summary>
    public TaskData(string title, string description, float baseReward, TypingTaskSO typingTaskData)
    {
        this.title = title;
        this.description = description;
        this.baseReward = baseReward;
        this.minigameType = typingTaskData != null ? MinigameType.Typing : MinigameType.None;
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
    /// Single pool - minigame type filters by day, upgrades provide progression
    /// </summary>
    private void InitializeDefaultTasks()
    {
        // Clear lists
        easyTasks.Clear();
        mediumTasks.Clear();
        hardTasks.Clear();

        // === TYPING TASKS ===
        AddTypingTask(easyTasks, "Draft Status Email", "Compose a brief status update", 15f);
        AddTypingTask(easyTasks, "Quick System Update", "Send quick notification", 12f);
        AddTypingTask(easyTasks, "Meeting Confirmation", "Confirm meeting attendance", 10f);
        AddTypingTask(easyTasks, "Daily Log Entry", "Record daily activities", 12f);
        AddTypingTask(easyTasks, "Backup Confirmation", "Confirm backup completion", 10f);
        AddTypingTask(mediumTasks, "Security Protocol Update", "Document new security measures", 65f);
        AddTypingTask(mediumTasks, "Meeting Minutes", "Record system status meeting", 60f);
        AddTypingTask(mediumTasks, "Performance Report", "Draft quarterly performance analysis", 70f);
        AddTypingTask(hardTasks, "Incident Report", "File detailed incident analysis", 250f);
        AddTypingTask(hardTasks, "System Architecture Document", "Draft comprehensive technical documentation", 280f);
        AddTypingTask(hardTasks, "Executive Summary", "Prepare executive briefing", 300f);

        // === MULTIPLE CHOICE TASKS ===
        AddMultipleChoiceTask(easyTasks, "File Format Question", "Which format is read-only?", 50f);
        AddMultipleChoiceTask(easyTasks, "Storage Question", "What is the backup size limit?", 45f);
        AddMultipleChoiceTask(mediumTasks, "Protocol Question", "Which protocol is most secure?", 200f);
        AddMultipleChoiceTask(mediumTasks, "Storage Limit Question", "Maximum file size per upload?", 190f);
        AddMultipleChoiceTask(hardTasks, "Authentication Question", "How many login attempts allowed?", 210f);

        // === MATH TASKS ===
        AddMathTask(easyTasks, "Addition Problem", "Solve the math problem", 220f);
        AddMathTask(easyTasks, "Subtraction Problem", "Solve the math problem", 230f);
        AddMathTask(mediumTasks, "Mixed Math", "Solve the math problem", 240f);
        AddMathTask(mediumTasks, "Complex Arithmetic", "Solve the math problem", 250f);

        // === PHOTO REVEAL TASKS ===
        AddPhotoRevealTask(easyTasks, "Identify Object", "Hover to reveal the image", 280f);
        AddPhotoRevealTask(mediumTasks, "Mosaic Puzzle", "Hover to reveal the hidden image", 290f);
        AddPhotoRevealTask(hardTasks, "Blur Challenge", "Hover to sharpen the image", 300f);

    }

    /// <summary>
    /// Add a multiple choice task
    /// </summary>
    private void AddMultipleChoiceTask(List<TaskData> taskPool, string title, string description, float reward)
    {
        TaskData taskData = new TaskData(title, description, reward, MinigameType.MultipleChoice);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Add a math task
    /// </summary>
    private void AddMathTask(List<TaskData> taskPool, string title, string description, float reward)
    {
        TaskData taskData = new TaskData(title, description, reward, MinigameType.Math);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Add a photo reveal task
    /// </summary>
    private void AddPhotoRevealTask(List<TaskData> taskPool, string title, string description, float reward)
    {
        TaskData taskData = new TaskData(title, description, reward, MinigameType.PhotoReveal);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Helper to add a typing task to a task pool
    /// </summary>
    private void AddTypingTask(List<TaskData> taskPool, string title, string description, float reward)
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
        TaskData taskData = new TaskData(title, description, reward, typingTask);
        taskPool.Add(taskData);
    }

    /// <summary>
    /// Get a random task from all available tasks
    /// </summary>
    public TaskData GetRandomTask()
    {
        EnsureInitialized();

        // Combine all pools
        List<TaskData> allTasks = new List<TaskData>();
        allTasks.AddRange(easyTasks);
        allTasks.AddRange(mediumTasks);
        allTasks.AddRange(hardTasks);

        if (allTasks.Count > 0)
        {
            int randomIndex = Random.Range(0, allTasks.Count);
            return allTasks[randomIndex];
        }

        // Fallback task if pool is empty
        return new TaskData("Generic Task", "Process outputs efficiently", 10f, MinigameType.None);
    }

    /// <summary>
    /// Get a random task based on current game day (gates minigame types - STACKING)
    /// Day 1: Typing
    /// Day 2: Typing + MultipleChoice
    /// Day 3: Typing + MultipleChoice + Math
    /// Day 4: Typing + MultipleChoice + Math + PhotoReveal
    /// Day 5: Typing (fallback)
    /// </summary>
    public TaskData GetRandomTaskForDay(int currentDay)
    {
        EnsureInitialized();

        // Combine all pools into one list
        List<TaskData> allTasks = new List<TaskData>();
        allTasks.AddRange(easyTasks);
        allTasks.AddRange(mediumTasks);
        allTasks.AddRange(hardTasks);


        // Filter by day (STACKING: each day adds to previous minigame types)
        List<TaskData> validTasks = new List<TaskData>();
        foreach (TaskData task in allTasks)
        {
            bool isValid = false;
            switch (currentDay)
            {
                case 1:
                    // Day 1: Typing only
                    isValid = (task.minigameType == MinigameType.Typing);
                    break;
                case 2:
                    // Day 2: Typing + MultipleChoice
                    isValid = (task.minigameType == MinigameType.Typing ||
                        task.minigameType == MinigameType.MultipleChoice);
                    break;
                case 3:
                    // Day 3: Typing + MultipleChoice + Math
                    isValid = (task.minigameType == MinigameType.Typing ||
                        task.minigameType == MinigameType.MultipleChoice ||
                        task.minigameType == MinigameType.Math);
                    break;
                case 4:
                    // Day 4: Typing + MultipleChoice + Math + PhotoReveal (all types)
                    isValid = (task.minigameType == MinigameType.Typing ||
                        task.minigameType == MinigameType.MultipleChoice ||
                        task.minigameType == MinigameType.Math ||
                        task.minigameType == MinigameType.PhotoReveal);
                    break;
                case 5:
                    // Day 5: Typing only (fallback, due to impossible quota)
                    isValid = (task.minigameType == MinigameType.Typing);
                    break;
                default:
                    isValid = false;
                    break;
            }

            if (isValid)
                validTasks.Add(task);
        }

        // Return random valid task, or fallback
        if (validTasks.Count > 0)
        {
            return validTasks[Random.Range(0, validTasks.Count)];
        }

        return GetRandomTask();
    }

}
