using UnityEngine;

public class GameHUDController : MonoBehaviour
{
    private GameHUDView _view;
    private GameState _gameState;

    public void Initialize(GameState gameState)
    {
        _gameState = gameState;
        _view = GetComponent<GameHUDView>();
        if (_view == null)
        {
            _view = gameObject.AddComponent<GameHUDView>();
        }

        // Subscribe to events
        GameEvents.Instance.OnDayChanged.AddListener(OnDayChanged);
        GameEvents.Instance.OnTasksCompletedChanged.AddListener(OnTasksChanged);
        GameEvents.Instance.OnDayTimerUpdated.AddListener(OnTimerUpdated);

        // Initial state
        UpdateUI();
    }

    private void OnDayChanged(int day)
    {
        _view.SetDay(day);
    }

    private void OnTasksChanged(int count)
    {
        _view.SetTasks(count, Constants.TASKS_REQUIRED_PER_DAY);
    }

    private void OnTimerUpdated(float timer)
    {
        float timeProgress = timer / Constants.DAY_DURATION;
        float displayTime = Constants.START_TIME + (Constants.END_TIME - Constants.START_TIME) * timeProgress;
        _view.SetTime(displayTime);

        float remaining = Constants.DAY_DURATION - timer;
        _view.SetTimer(remaining);
    }

    private void UpdateUI()
    {
        _view.SetDay(_gameState.Current.CurrentDay);
        _view.SetTasks(_gameState.Current.TasksCompleted, Constants.TASKS_REQUIRED_PER_DAY);
        OnTimerUpdated(_gameState.Current.DayTimer);
    }
}
