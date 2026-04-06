using UnityEngine;

/// <summary>
/// Singleton tracking all purchased upgrades across days.
/// Minigames query this to apply upgrade effects.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // Day 1 upgrades
    [SerializeField] private bool fasterTypingActive = false;
    [SerializeField] private bool autoStampActive = false;

    // Day 2 upgrades
    [SerializeField] private bool numberLockActive = false;
    [SerializeField] private bool batchProcessActive = false;

    // Day 3 upgrades
    [SerializeField] private bool preSortedActive = false;
    [SerializeField] private bool quickScanActive = false;

    // Day 4 upgrades
    [SerializeField] private bool overclockActive = false;
    [SerializeField] private bool memoryAssistActive = false;

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

    // Day 1 Upgrade Setters
    public void ActivateFasterTyping() => fasterTypingActive = true;
    public void ActivateAutoStamp() => autoStampActive = true;

    // Day 2 Upgrade Setters
    public void ActivateNumberLock() => numberLockActive = true;
    public void ActivateBatchProcess() => batchProcessActive = true;

    // Day 3 Upgrade Setters
    public void ActivatePreSorted() => preSortedActive = true;
    public void ActivateQuickScan() => quickScanActive = true;

    // Day 4 Upgrade Setters
    public void ActivateOverclock() => overclockActive = true;
    public void ActivateMemoryAssist() => memoryAssistActive = true;

    // Upgrade Getters
    public bool IsFasterTypingActive() => fasterTypingActive;
    public bool IsAutoStampActive() => autoStampActive;
    public bool IsNumberLockActive() => numberLockActive;
    public bool IsBatchProcessActive() => batchProcessActive;
    public bool IsPreSortedActive() => preSortedActive;
    public bool IsQuickScanActive() => quickScanActive;
    public bool IsOverclockActive() => overclockActive;
    public bool IsMemoryAssistActive() => memoryAssistActive;
}
