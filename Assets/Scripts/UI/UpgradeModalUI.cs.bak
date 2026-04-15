using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI for day-end upgrade selection modal.
/// Displays two upgrade choices for the player to pick one before advancing to next day.
/// </summary>
public class UpgradeModalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private Button rightButton;

    private System.Action onUpgradeChosen;

    public void SetupForDay(int day, System.Action onChosen)
    {
        onUpgradeChosen = onChosen;

        if (titleText != null)
        {
            titleText.text = $"Day {day} - Choose an Upgrade";
        }

        leftButton.onClick.AddListener(OnLeftChosen);
        rightButton.onClick.AddListener(OnRightChosen);

        // Set button texts based on day
        switch (day)
        {
            case 1:
                SetButtonTexts("Faster Typing\n(Fewer key presses)", "Auto-Stamp\n(No dragging)");
                break;
            case 2:
                SetButtonTexts("Number Lock\n(Show answer)", "Batch Process\n(2x processing)");
                break;
            case 3:
                SetButtonTexts("Pre-Sorted\n(Files start sorted)", "Quick Scan\n(Highlight error)");
                break;
            case 4:
                SetButtonTexts("Overclock\n(Faster printer)", "Memory Assist\n(Show hints)");
                break;
        }
    }

    private void SetButtonTexts(string left, string right)
    {
        if (leftButtonText != null) leftButtonText.text = left;
        if (rightButtonText != null) rightButtonText.text = right;
    }

    private void OnLeftChosen()
    {
        int day = GameManager.Instance?.GetCurrentDay() ?? 1;

        switch (day)
        {
            case 1:
                UpgradeManager.Instance?.ActivateFasterTyping();
                break;
            case 2:
                UpgradeManager.Instance?.ActivateNumberLock();
                break;
            case 3:
                UpgradeManager.Instance?.ActivatePreSorted();
                break;
            case 4:
                UpgradeManager.Instance?.ActivateOverclock();
                break;
        }

        onUpgradeChosen?.Invoke();
    }

    private void OnRightChosen()
    {
        int day = GameManager.Instance?.GetCurrentDay() ?? 1;

        switch (day)
        {
            case 1:
                UpgradeManager.Instance?.ActivateAutoStamp();
                break;
            case 2:
                UpgradeManager.Instance?.ActivateBatchProcess();
                break;
            case 3:
                UpgradeManager.Instance?.ActivateQuickScan();
                break;
            case 4:
                UpgradeManager.Instance?.ActivateMemoryAssist();
                break;
        }

        onUpgradeChosen?.Invoke();
    }
}
