using UnityEngine;

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
