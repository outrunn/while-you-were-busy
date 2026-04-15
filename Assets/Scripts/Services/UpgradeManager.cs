using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private UpgradeProgress _progress = new UpgradeProgress();

    private static readonly Upgrade[] _upgradeDatabase = new Upgrade[]
    {
        // Day 1 upgrades
        new Upgrade(UpgradeType.FasterTyping, "Faster Typing", "Fewer key presses required", 1),
        new Upgrade(UpgradeType.AutoStamp, "Auto-Stamp", "No dragging required", 1),

        // Day 2 upgrades
        new Upgrade(UpgradeType.NumberLock, "Number Lock", "Show answer hints", 2),
        new Upgrade(UpgradeType.BatchProcess, "Batch Process", "2x processing speed", 2),

        // Day 3 upgrades
        new Upgrade(UpgradeType.PreSorted, "Pre-Sorted", "Files start sorted", 3),
        new Upgrade(UpgradeType.QuickScan, "Quick Scan", "Highlight errors automatically", 3),

        // Day 4 upgrades
        new Upgrade(UpgradeType.Overclock, "Overclock", "Faster printer speed", 4),
        new Upgrade(UpgradeType.MemoryAssist, "Memory Assist", "Show memory hints", 4),
    };

    public Upgrade GetUpgradeByType(UpgradeType type)
    {
        foreach (var upgrade in _upgradeDatabase)
        {
            if (upgrade.Type == type)
                return upgrade;
        }
        return null;
    }

    public Upgrade[] GetUpgradesForDay(int day)
    {
        System.Collections.Generic.List<Upgrade> dayUpgrades = new System.Collections.Generic.List<Upgrade>();
        foreach (var upgrade in _upgradeDatabase)
        {
            if (upgrade.Day == day)
                dayUpgrades.Add(upgrade);
        }
        return dayUpgrades.ToArray();
    }

    public void PurchaseUpgrade(UpgradeType upgradeType)
    {
        _progress.PurchaseUpgrade(upgradeType);
        Debug.Log($"Upgrade purchased: {upgradeType}");
        GameEvents.Instance?.OnUpgradePurchased?.Invoke(upgradeType);
    }

    public bool IsUpgradePurchased(UpgradeType upgradeType)
    {
        return _progress.IsUpgradePurchased(upgradeType);
    }

    public UpgradeProgress GetProgress()
    {
        return _progress;
    }

    public void ResetProgress()
    {
        _progress.Reset();
    }
}
