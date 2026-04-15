using System.Collections.Generic;
using UnityEngine;

public class TaskService
{
    private List<TaskData> _easyTasks = new List<TaskData>();
    private List<TaskData> _mediumTasks = new List<TaskData>();
    private List<TaskData> _hardTasks = new List<TaskData>();
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
        TaskData task = new TaskData(title, description, reward, typingTask);
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
        List<TaskData> allTasks = new List<TaskData>();
        allTasks.AddRange(_easyTasks);
        allTasks.AddRange(_mediumTasks);
        allTasks.AddRange(_hardTasks);

        List<TaskData> validTasks = new List<TaskData>();
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
        List<TaskData> allTasks = new List<TaskData>();
        allTasks.AddRange(_easyTasks);
        allTasks.AddRange(_mediumTasks);
        allTasks.AddRange(_hardTasks);

        if (allTasks.Count > 0)
            return allTasks[Random.Range(0, allTasks.Count)];

        return new TaskData("Generic Task", "Process outputs efficiently", 10f, MinigameType.None);
    }
}
