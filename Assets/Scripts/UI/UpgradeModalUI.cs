using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeModalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private Button rightButton;

    private int _currentDay;
    private System.Action _onClose;

    public void Initialize(int day)
    {
        _currentDay = day;

        if (titleText != null)
            titleText.text = $"Day {day} - Choose an Upgrade";

        leftButton.onClick.AddListener(OnLeftChosen);
        rightButton.onClick.AddListener(OnRightChosen);

        SetButtonTexts(day);
    }

    private void SetButtonTexts(int day)
    {
        string left = "", right = "";

        switch (day)
        {
            case 1:
                left = "Faster Typing\n(Fewer key presses)";
                right = "Auto-Stamp\n(No dragging)";
                break;
            case 2:
                left = "Number Lock\n(Show answer)";
                right = "Batch Process\n(2x processing)";
                break;
            case 3:
                left = "Pre-Sorted\n(Files start sorted)";
                right = "Quick Scan\n(Highlight error)";
                break;
            case 4:
                left = "Overclock\n(Faster printer)";
                right = "Memory Assist\n(Show hints)";
                break;
        }

        if (leftButtonText != null)
            leftButtonText.text = left;
        if (rightButtonText != null)
            rightButtonText.text = right;
    }

    private void OnLeftChosen()
    {
        // Upgrade logic would be tied to GameService/UpgradeService here
        Debug.Log($"Day {_currentDay} left upgrade chosen");
        CloseModal();
    }

    private void OnRightChosen()
    {
        // Upgrade logic would be tied to GameService/UpgradeService here
        Debug.Log($"Day {_currentDay} right upgrade chosen");
        CloseModal();
    }

    public void CloseModal()
    {
        _onClose?.Invoke();
        gameObject.SetActive(false);
    }
}
