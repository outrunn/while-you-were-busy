using UnityEngine;

/// <summary>
/// ScriptableObject that defines a typing minigame task.
/// Contains the message the player needs to "type" and associated metadata.
/// </summary>
[CreateAssetMenu(fileName = "New Typing Task", menuName = "Minigames/Typing Task")]
public class TypingTaskSO : ScriptableObject
{
    [Header("Task Information")]
    [Tooltip("The title of this typing task (e.g., 'Draft Server Report')")]
    public string taskTitle = "Typing Task";

    [TextArea(3, 6)]
    [Tooltip("Optional flavor text describing what the player is typing")]
    public string taskDescription = "Complete this typing task";

    [Header("Typing Content")]
    [TextArea(4, 10)]
    [Tooltip("The message that will be typed out when the player spams keys")]
    public string messageToType = "Enter your message here...";

    [Header("Minigame Settings")]
    [Tooltip("Minimum number of key presses required (0 = auto-calculate from message length)")]
    public int minimumKeyPresses = 0;

    [Tooltip("Delay in seconds before auto-closing the window after completion")]
    public float completionDelay = 2.5f;

    /// <summary>
    /// Gets the actual number of key presses required for this task.
    /// Uses minimumKeyPresses if set, otherwise uses message character count.
    /// </summary>
    public int GetRequiredKeyPresses()
    {
        return minimumKeyPresses > 0 ? minimumKeyPresses : messageToType.Length;
    }

    /// <summary>
    /// Validates the typing task data in the Unity Inspector.
    /// </summary>
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(messageToType))
        {
            Debug.LogWarning($"TypingTaskSO '{name}' has no message to type!", this);
        }

        if (minimumKeyPresses < 0)
        {
            minimumKeyPresses = 0;
        }

        if (completionDelay < 0)
        {
            completionDelay = 0;
        }
    }
}
