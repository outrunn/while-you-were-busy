using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles upgrade button display and purchase logic.
/// Displays upgrade name, cost, and handles purchase attempts.
/// </summary>
public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        AutoProcessor,
        ScalingEngine,
        ExpansionProtocol
    }

    [Header("Upgrade Settings")]
    [SerializeField] private UpgradeType upgradeType;

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private Text nameText;
    [SerializeField] private Text costText;
    [SerializeField] private Text descriptionText;

    [Header("Upgrade Display Info")]
    [SerializeField] private string upgradeName;
    [SerializeField] private string upgradeDescription;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnPurchaseAttempt);
        }

        // Set static display text
        if (nameText != null)
        {
            nameText.text = upgradeName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = upgradeDescription;
        }
    }

    private void Update()
    {
        UpdateCostDisplay();
        UpdateButtonInteractable();
    }

    /// <summary>
    /// Called when player clicks the upgrade button
    /// </summary>
    private void OnPurchaseAttempt()
    {
        if (GameManager.Instance == null) return;

        bool success = false;

        switch (upgradeType)
        {
            case UpgradeType.AutoProcessor:
                success = GameManager.Instance.PurchaseAutoProcessor();
                break;
            case UpgradeType.ScalingEngine:
                success = GameManager.Instance.PurchaseScalingEngine();
                break;
            case UpgradeType.ExpansionProtocol:
                success = GameManager.Instance.PurchaseExpansionProtocol();
                break;
        }

        if (success)
        {
            // Optional: Add purchase sound or effect here
        }
    }

    /// <summary>
    /// Update cost display text
    /// </summary>
    private void UpdateCostDisplay()
    {
        if (costText == null || GameManager.Instance == null) return;

        float cost = GetCurrentCost();
        costText.text = $"Cost: {cost:F0}";
    }

    /// <summary>
    /// Enable/disable button based on whether player can afford it
    /// </summary>
    private void UpdateButtonInteractable()
    {
        if (button == null || GameManager.Instance == null) return;

        float cost = GetCurrentCost();
        float currentOutputs = GameManager.Instance.GetOutputs();

        button.interactable = currentOutputs >= cost;
    }

    /// <summary>
    /// Get the current cost of this upgrade
    /// </summary>
    private float GetCurrentCost()
    {
        if (GameManager.Instance == null) return 0f;

        switch (upgradeType)
        {
            case UpgradeType.AutoProcessor:
                return GameManager.Instance.GetAutoProcessorCost();
            case UpgradeType.ScalingEngine:
                return GameManager.Instance.GetScalingEngineCost();
            case UpgradeType.ExpansionProtocol:
                return GameManager.Instance.GetExpansionProtocolCost();
            default:
                return 0f;
        }
    }
}
