using UnityEngine;

[System.Serializable]
public class GameStateData
{
    public int CurrentDay { get; private set; }
    public int TasksCompleted { get; private set; }
    public float DayTimer { get; private set; }
    public float WorldHealth { get; private set; }
    public float Outputs { get; private set; }

    public GameStateData()
    {
        CurrentDay = 1;
        TasksCompleted = 0;
        DayTimer = 0f;
        WorldHealth = Constants.INITIAL_WORLD_HEALTH;
        Outputs = 0f;
    }

    // Return new instance with updated values (immutable pattern)
    public GameStateData WithDay(int day) => new GameStateData()
    {
        CurrentDay = day,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithTasksCompleted(int count) => new GameStateData()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = count,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithDayTimer(float timer) => new GameStateData()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = timer,
        WorldHealth = WorldHealth,
        Outputs = Outputs
    };

    public GameStateData WithWorldHealth(float health) => new GameStateData()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = Mathf.Max(0, health),
        Outputs = Outputs
    };

    public GameStateData WithOutputs(float outputs) => new GameStateData()
    {
        CurrentDay = CurrentDay,
        TasksCompleted = TasksCompleted,
        DayTimer = DayTimer,
        WorldHealth = WorldHealth,
        Outputs = outputs
    };
}

[System.Serializable]
public class GameState
{
    private GameStateData _data = new GameStateData();

    public GameStateData Current => _data;

    public void Update(GameStateData newData) => _data = newData;

    public void Reset() => _data = new GameStateData();
}
