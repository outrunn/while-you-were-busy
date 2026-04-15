using UnityEngine;
using UnityEngine.Events;

public enum MinigameType { None, Typing, Math, MultipleChoice, PhotoReveal }

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    // Game state changes
    public UnityEvent<int> OnDayChanged = new UnityEvent<int>();
    public UnityEvent<int> OnTasksCompletedChanged = new UnityEvent<int>();
    public UnityEvent<float> OnDayTimerUpdated = new UnityEvent<float>();
    public UnityEvent<float> OnWorldHealthChanged = new UnityEvent<float>();
    public UnityEvent<float> OnOutputsChanged = new UnityEvent<float>();

    // Day lifecycle
    public UnityEvent OnDayStarted = new UnityEvent();
    public UnityEvent OnDayEnded = new UnityEvent();
    public UnityEvent OnGameWon = new UnityEvent();
    public UnityEvent OnGameLost = new UnityEvent();

    // Ticket lifecycle
    public UnityEvent<TicketModel> OnTicketSpawned = new UnityEvent<TicketModel>();
    public UnityEvent<TicketModel> OnTicketStamped = new UnityEvent<TicketModel>();
    public UnityEvent<TicketModel> OnTicketShredded = new UnityEvent<TicketModel>();

    // Minigame lifecycle
    public UnityEvent<MinigameType> OnMinigameStarted = new UnityEvent<MinigameType>();
    public UnityEvent<MinigameType> OnMinigameCompleted = new UnityEvent<MinigameType>();
    public UnityEvent<MinigameType> OnMinigameFailed = new UnityEvent<MinigameType>();

    // Upgrade
    public UnityEvent<int> OnUpgradeApplied = new UnityEvent<int>();
    public UnityEvent<UpgradeType> OnUpgradePurchased = new UnityEvent<UpgradeType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
