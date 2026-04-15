using UnityEngine;
using TMPro;

public class GameHUDView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI tasksText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void OnEnable()
    {
        // Auto-find text components if not assigned
        if (dayText == null)
            dayText = transform.Find("DayText")?.GetComponent<TextMeshProUGUI>();
        if (tasksText == null)
            tasksText = transform.Find("TasksText")?.GetComponent<TextMeshProUGUI>();
        if (timeText == null)
            timeText = transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        if (timerText == null)
            timerText = transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
    }

    public void SetDay(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {day}";
    }

    public void SetTasks(int completed, int required)
    {
        if (tasksText != null)
            tasksText.text = $"Tasks: {completed}/{required}";
    }

    public void SetTime(float hours)
    {
        if (timeText != null)
            timeText.text = $"Time: {FormatTime(hours)}";
    }

    public void SetTimer(float remaining)
    {
        if (timerText != null)
            timerText.text = $"Time left: {remaining:F1}s";
    }

    private string FormatTime(float hours)
    {
        int hour = Mathf.FloorToInt(hours);
        int minute = Mathf.FloorToInt((hours - hour) * 60f);
        string period = hour >= 12 ? "PM" : "AM";
        int displayHour = hour % 12;
        if (displayHour == 0) displayHour = 12;
        return $"{displayHour}:{minute:D2} {period}";
    }
}
