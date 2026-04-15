using UnityEngine;

public class TypingMinigame : MonoBehaviour, IMinigame
{
    public MinigameType Type => MinigameType.Typing;
    public bool IsActive { get; private set; }
    public bool IsCompleted { get; private set; }

    private TypingMinigameUI _ui;
    private System.Action _onComplete;

    private void Awake()
    {
        _ui = GetComponent<TypingMinigameUI>();
        if (_ui == null)
            _ui = gameObject.AddComponent<TypingMinigameUI>();
    }

    public void SetTask(TypingTaskSO task)
    {
        if (_ui != null)
            _ui.SetTask(task);
    }

    void IMinigame.Start(System.Action onComplete)
    {
        _onComplete = onComplete;
        IsActive = true;
        IsCompleted = false;

        if (_ui != null)
        {
            _ui.StartMinigame(() =>
            {
                IsCompleted = true;
                IsActive = false;
                _onComplete?.Invoke();
            });
        }
    }

    public void Close()
    {
        IsActive = false;
        if (_ui != null)
            _ui.CloseMinigame();
        Destroy(gameObject);
    }
}
