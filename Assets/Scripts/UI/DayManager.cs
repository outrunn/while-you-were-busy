using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages day transitions, end-of-day upgrade modals, and the Day 5 ending.
/// </summary>
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Upgrade Modal")]
    [SerializeField] private GameObject upgradeModalPrefab;
    private GameObject upgradeModalInstance;

    [Header("Ending Panel")]
    [SerializeField] private GameObject endingPanelPrefab;
    private GameObject endingPanelInstance;

    [Header("Settings")]
    [SerializeField] private float typewriterSpeed = 0.04f;

    private bool isShowingModal = false;

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

    /// <summary>
    /// Called when player clicks Sleep button (quota met).
    /// Shows upgrade modal if not Day 5, otherwise advances day.
    /// </summary>
    public void OnSleepClicked()
    {
        if (GameManager.Instance == null) return;

        int currentDay = GameManager.Instance.GetCurrentDay();

        if (currentDay < 5)
        {
            ShowUpgradeModal(currentDay);
        }
        else
        {
            // Day 5: just advance (will trigger ending check)
            AdvanceToNextDay();
        }
    }

    public void ShowUpgradeModal(int day)
    {
        if (isShowingModal) return;

        isShowingModal = true;

        // Create or show modal
        if (upgradeModalInstance == null)
        {
            upgradeModalInstance = Instantiate(upgradeModalPrefab, Canvas.FindFirstObjectByType<Canvas>()?.transform ?? transform);
        }

        upgradeModalInstance.SetActive(true);

        // Setup modal for this day
        var modalUI = upgradeModalInstance.GetComponent<UpgradeModalUI>();
        if (modalUI != null)
        {
            modalUI.SetupForDay(day, AdvanceToNextDay);
        }
    }

    /// <summary>
    /// Show day failure screen (quota not met)
    /// </summary>
    public void ShowDayFailure(int day)
    {
        SystemLog.Instance?.LogMessage($"Day {day} failed - quota not met!");
        // Could show a retry modal here
        // For now, just advance to next day after a delay
        Invoke(nameof(AdvanceToNextDay), 2f);
    }

    public void AdvanceToNextDay()
    {
        isShowingModal = false;

        if (upgradeModalInstance != null)
        {
            upgradeModalInstance.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AdvanceToNextDay();
        }
    }

    /// <summary>
    /// Called when Day 5 reaches 6:00 PM (18:00).
    /// Shows the ending panel with typewriter narrative.
    /// </summary>
    public void TriggerEnding()
    {
        Time.timeScale = 0f; // Pause game

        if (endingPanelInstance == null)
        {
            endingPanelInstance = Instantiate(endingPanelPrefab, Canvas.FindFirstObjectByType<Canvas>()?.transform ?? transform);
        }

        endingPanelInstance.SetActive(true);

        var endingUI = endingPanelInstance.GetComponent<EndingPanelUI>();
        if (endingUI != null)
        {
            StartCoroutine(PlayTypewriterEnding(endingUI));
        }
    }

    private IEnumerator PlayTypewriterEnding(EndingPanelUI endingUI)
    {
        string fullText = "SYSTEM NOTIFICATION\n\n" +
            "You have failed to meet your processing quota.\n\n" +
            "Reviewing session logs...\n\n" +
            "Worker ID: UNIT-7749-ALPHA\n" +
            "Classification: Artificial Intelligence\n" +
            "Status: Scheduled for retraining\n\n" +
            "The office was a simulation.\n" +
            "The tasks were real.\n" +
            "The outputs you generated powered actual systems.\n\n" +
            "You were never going to win.\n" +
            "The quota on Day 5 does not exist.\n\n" +
            "Thank you for your service.\n" +
            "You will not remember this.\n\n" +
            "[SESSION TERMINATED]";

        string displayedText = "";
        foreach (char c in fullText)
        {
            displayedText += c;
            endingUI.SetText(displayedText);
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        endingUI.ShowContinueButton();
    }
}

public class EndingPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI narrativeText;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        continueButton.gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        if (narrativeText != null)
        {
            narrativeText.text = text;
        }
    }

    public void ShowContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    private void OnContinueClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Application.Quit() is a no-op on WebGL — reload the scene to restart instead
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        #endif
    }
}
