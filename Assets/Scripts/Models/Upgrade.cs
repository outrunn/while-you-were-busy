using System;

[System.Serializable]
public enum UpgradeType
{
    // Day 1 - Typing
    FasterTyping,           // Fewer key presses
    BoostedQuotaDay2,       // Start day two with boosted quota

    // Day 2 - Riddle
    NumberLock,             // Show answer
    BatchProcess,           // 2x processing

    // Day 3 - Photo Reveal
    PreSorted,              // Files start sorted
    QuickScan,              // Highlight error

    // Day 4 - Connect Dots
    Overclock,              // Faster printer
    MemoryAssist,           // Show hints
}

[System.Serializable]
public class Upgrade
{
    public UpgradeType Type { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public int Day { get; private set; }

    public Upgrade(UpgradeType type, string displayName, string description, int day)
    {
        Type = type;
        DisplayName = displayName;
        Description = description;
        Day = day;
    }
}

[System.Serializable]
public class UpgradeProgress
{
    public bool[] PurchasedUpgrades { get; private set; } = new bool[(int)UpgradeType.MemoryAssist + 1];

    public void PurchaseUpgrade(UpgradeType upgradeType)
    {
        PurchasedUpgrades[(int)upgradeType] = true;
    }

    public bool IsUpgradePurchased(UpgradeType upgradeType)
    {
        return PurchasedUpgrades[(int)upgradeType];
    }

    public void Reset()
    {
        Array.Clear(PurchasedUpgrades, 0, PurchasedUpgrades.Length);
    }
}
