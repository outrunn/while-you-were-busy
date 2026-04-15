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
    [SerializeField] private CanvasGroup canvasGroup;

    private int _currentDay;
    private UpgradeType _leftUpgradeType;
    private UpgradeType _rightUpgradeType;
    private UpgradeManager _upgradeManager;
    private System.Action _onClose;

    public void Initialize(int day, UpgradeManager upgradeManager, System.Action onClose)
    {
        _currentDay = day;
        _upgradeManager = upgradeManager;
        _onClose = onClose;

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (titleText != null)
            titleText.text = $"Day {day} Complete - Choose an Upgrade";

        // Clear previous listeners
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(OnLeftChosen);
        rightButton.onClick.AddListener(OnRightChosen);

        SetUpgradeOptions(day);
    }

    private void SetUpgradeOptions(int day)
    {
        var upgradesForDay = _upgradeManager.GetUpgradesForDay(day);

        if (upgradesForDay.Length >= 2)
        {
            var leftUpgrade = upgradesForDay[0];
            var rightUpgrade = upgradesForDay[1];

            _leftUpgradeType = leftUpgrade.Type;
            _rightUpgradeType = rightUpgrade.Type;

            if (leftButtonText != null)
                leftButtonText.text = $"{leftUpgrade.DisplayName}\n({leftUpgrade.Description})";
            if (rightButtonText != null)
                rightButtonText.text = $"{rightUpgrade.DisplayName}\n({rightUpgrade.Description})";
        }
    }

    private void OnLeftChosen()
    {
        if (_upgradeManager != null)
        {
            _upgradeManager.PurchaseUpgrade(_leftUpgradeType);
        }
        CloseModal();
    }

    private void OnRightChosen()
    {
        if (_upgradeManager != null)
        {
            _upgradeManager.PurchaseUpgrade(_rightUpgradeType);
        }
        CloseModal();
    }

    public void CloseModal()
    {
        _onClose?.Invoke();
        gameObject.SetActive(false);
    }
}
