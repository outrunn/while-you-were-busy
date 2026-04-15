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
        // Load prefab from Resources/Prefabs/Minigames/
        string prefabPath = $"Prefabs/Minigames/{type.ToString()}Minigame";
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
