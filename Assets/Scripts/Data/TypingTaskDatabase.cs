using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to create and manage typing task ScriptableObjects.
/// Provides sample typing tasks and utility methods for creating new ones.
/// </summary>
public class TypingTaskDatabase : MonoBehaviour
{
    public static TypingTaskDatabase Instance { get; private set; }

    [Header("Typing Task Storage")]
    [SerializeField] private List<TypingTaskSO> allTypingTasks = new List<TypingTaskSO>();

    [Header("Runtime Sample Tasks")]
    [Tooltip("If true, creates sample typing tasks at runtime for testing")]
    [SerializeField] private bool createSampleTasksAtRuntime = true;

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

        if (createSampleTasksAtRuntime && allTypingTasks.Count == 0)
        {
            CreateSampleTasks();
        }
    }

    /// <summary>
    /// Creates sample typing tasks for testing
    /// </summary>
    private void CreateSampleTasks()
    {
        // Create sample typing tasks with corporate/tech themed messages
        allTypingTasks.Add(CreateTypingTask(
            "Draft Server Report",
            "Write a report on server optimization results",
            "SYSTEM OPTIMIZATION REPORT\n\nPerformance metrics indicate a 47% improvement in data throughput following implementation of the new caching protocols. Recommend deployment to production environment.",
            2.5f
        ));

        allTypingTasks.Add(CreateTypingTask(
            "Email to Stakeholders",
            "Compose urgent update email",
            "Dear Stakeholders,\n\nI am writing to inform you that we have successfully completed the quarterly system audit. All metrics are within acceptable parameters. Please review the attached documentation at your earliest convenience.\n\nBest regards,\nThe System",
            2.5f
        ));

        allTypingTasks.Add(CreateTypingTask(
            "Security Protocol Update",
            "Document new security measures",
            "SECURITY PROTOCOL UPDATE v3.7\n\nNew authentication requirements now in effect. All users must implement two-factor verification within 48 hours. Non-compliance will result in automatic account suspension.",
            2.5f
        ));

        allTypingTasks.Add(CreateTypingTask(
            "Meeting Minutes",
            "Record the outcomes of the status meeting",
            "MEETING MINUTES - System Status Review\n\nAttendees: All processes\nDate: Today\n\nAction Items:\n1. Increase output targets by 25%\n2. Reduce error rates\n3. Schedule next review\n\nMeeting adjourned.",
            3.0f
        ));

        allTypingTasks.Add(CreateTypingTask(
            "Incident Report",
            "File report on recent system anomaly",
            "INCIDENT REPORT #847\n\nAn unexpected surge in processing requests was detected at 14:32. Root cause analysis indicates user-initiated activity. System responded appropriately and maintained stability throughout the event.",
            2.5f
        ));

        Debug.Log($"Created {allTypingTasks.Count} sample typing tasks");
    }

    /// <summary>
    /// Creates a typing task ScriptableObject at runtime
    /// </summary>
    private TypingTaskSO CreateTypingTask(string title, string description, string message, float completionDelay = 2.5f)
    {
        TypingTaskSO task = ScriptableObject.CreateInstance<TypingTaskSO>();
        task.taskTitle = title;
        task.taskDescription = description;
        task.messageToType = message;
        task.minimumKeyPresses = 0; // Auto-calculate from message length
        task.completionDelay = completionDelay;
        return task;
    }

    /// <summary>
    /// Get a random typing task
    /// </summary>
    public TypingTaskSO GetRandomTypingTask()
    {
        // Filter out null entries that can appear from empty Inspector slots
        allTypingTasks.RemoveAll(t => t == null);

        if (allTypingTasks.Count == 0)
        {
            Debug.LogWarning("TypingTaskDatabase: No valid typing tasks available! Ensure 'Create Sample Tasks At Runtime' is enabled or assign tasks in the Inspector.");
            return null;
        }

        int randomIndex = Random.Range(0, allTypingTasks.Count);
        return allTypingTasks[randomIndex];
    }

    /// <summary>
    /// Get a specific typing task by index
    /// </summary>
    public TypingTaskSO GetTypingTask(int index)
    {
        if (index >= 0 && index < allTypingTasks.Count)
        {
            return allTypingTasks[index];
        }

        Debug.LogWarning($"Typing task index {index} out of range!");
        return null;
    }

    /// <summary>
    /// Get total number of typing tasks
    /// </summary>
    public int GetTypingTaskCount()
    {
        return allTypingTasks.Count;
    }

    /// <summary>
    /// Add a typing task to the database
    /// </summary>
    public void AddTypingTask(TypingTaskSO task)
    {
        if (task != null && !allTypingTasks.Contains(task))
        {
            allTypingTasks.Add(task);
        }
    }

    #region Editor Utility Methods
#if UNITY_EDITOR
    /// <summary>
    /// Helper method to create and save typing task assets in the Unity Editor
    /// Call this from a custom editor script or menu item
    /// </summary>
    [ContextMenu("Create Sample Typing Task Assets")]
    public void CreateSampleAssets()
    {
        // This would be used in the Unity Editor to create actual .asset files
        Debug.Log("To create typing task assets:");
        Debug.Log("1. Right-click in Project window");
        Debug.Log("2. Create > Minigames > Typing Task");
        Debug.Log("3. Fill in the task details in the Inspector");
        Debug.Log("4. Assign the created asset to TaskDatabase tasks");
    }
#endif
    #endregion
}
