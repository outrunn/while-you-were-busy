using System;

public interface IMinigame
{
    MinigameType Type { get; }
    bool IsActive { get; }
    bool IsCompleted { get; }

    void Start(Action onComplete);
    void Close();
}
