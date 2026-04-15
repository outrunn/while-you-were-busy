using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Simple keyboard debug commands. Just press keys to test things.
/// No UI setup needed - just attach to any GameObject and go.
/// Only available in Editor builds.
/// </summary>
public class DebugCommands : MonoBehaviour
{
    private void Update()
    {
        if (!Input.anyKeyDown) return;

        // Output commands
        if (Input.GetKeyDown(KeyCode.O)) AddOutputs(50);
        if (Input.GetKeyDown(KeyCode.P)) AddOutputs(100);
        if (Input.GetKeyDown(KeyCode.I)) CompleteQuota();

        // Day commands
        if (Input.GetKeyDown(KeyCode.N)) AdvanceDay();
        if (Input.GetKeyDown(KeyCode.F)) SetDay(5);

        // Ticket commands
        if (Input.GetKeyDown(KeyCode.T)) PrintTicket();
        if (Input.GetKeyDown(KeyCode.M)) StartMinigame();

        // Health commands
        if (Input.GetKeyDown(KeyCode.H)) DamageHealth(50);
        if (Input.GetKeyDown(KeyCode.E)) TriggerEnding();

        // Log state
        if (Input.GetKeyDown(KeyCode.S)) LogState();
    }

    private void AddOutputs(float amount)
    {
        GameManager.Instance?.AddOutputs(amount);
        Debug.Log($"[DEBUG] +{amount} outputs");
    }

    private void CompleteQuota()
    {
        if (GameManager.Instance == null) return;
        float needed = GameManager.Instance.GetDailyQuota() - GameManager.Instance.GetDailyPoints();
        GameManager.Instance.AddOutputs(needed);
        Debug.Log("[DEBUG] Quota completed");
    }

    private void AdvanceDay()
    {
        GameManager.Instance?.AdvanceToNextDay();
        Debug.Log($"[DEBUG] Advanced to Day {GameManager.Instance?.GetCurrentDay()}");
    }

    private void SetDay(int day)
    {
        if (GameManager.Instance == null) return;
        while (GameManager.Instance.GetCurrentDay() < day)
        {
            GameManager.Instance.AdvanceToNextDay();
        }
        Debug.Log($"[DEBUG] Set to Day {day}");
    }

    private void PrintTicket()
    {
        var printer = FindFirstObjectByType<Printer>();
        if (printer != null)
        {
            printer.PrintTicket();
            Debug.Log("[DEBUG] Ticket printed");
        }
    }

    private void StartMinigame()
    {
        var tickets = FindObjectsByType<Ticket>(FindObjectsSortMode.None);
        foreach (var ticket in tickets)
        {
            if (!ticket.IsStamped())
            {
                ticket.GetComponent<UnityEngine.UI.Button>()?.onClick.Invoke();
                Debug.Log("[DEBUG] Started minigame");
                return;
            }
        }
        Debug.Log("[DEBUG] No unstamped tickets");
    }

    private void DamageHealth(float amount)
    {
        GameManager.Instance?.DegradeWorldHealth(amount);
        Debug.Log($"[DEBUG] Health -{amount}");
    }

    private void TriggerEnding()
    {
        DayManager.Instance?.TriggerEnding();
        Debug.Log("[DEBUG] Ending triggered");
    }

    private void LogState()
    {
        if (GameManager.Instance == null) return;
        Debug.Log($"[STATE] Day {GameManager.Instance.GetCurrentDay()} | " +
                  $"Quota {GameManager.Instance.GetDailyPoints():F0}/{GameManager.Instance.GetDailyQuota():F0} | " +
                  $"Outputs {GameManager.Instance.GetOutputs():F0} | " +
                  $"Health {GameManager.Instance.GetWorldHealth():F0}");
    }
}
#endif
