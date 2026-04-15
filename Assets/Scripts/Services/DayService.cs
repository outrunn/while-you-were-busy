using System.Collections;
using UnityEngine;
using TMPro;

public class DayService : MonoBehaviour
{
    private static readonly string UPGRADE_MODAL_PATH = "Prefabs/UI/UpgradeModal";

    [SerializeField] private GameObject endingPanelPrefab;
    [SerializeField] private Canvas canvas;

    private GameObject _upgradeModalInstance;
    private GameObject _endingPanelInstance;
    private UpgradeManager _upgradeManager;
    private GameObject _upgradeModalPrefab;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        _upgradeManager = FindFirstObjectByType<UpgradeManager>();

        // Load upgrade modal prefab from Resources
        _upgradeModalPrefab = Resources.Load<GameObject>(UPGRADE_MODAL_PATH);
        if (_upgradeModalPrefab == null)
        {
            Debug.LogError($"[DayService] Failed to load upgrade modal prefab from {UPGRADE_MODAL_PATH}");
        }
    }

    public void ShowUpgradeModal(int completedDay)
    {
        if (_upgradeManager == null)
        {
            Debug.LogError("UpgradeManager not found in scene!");
            return;
        }

        if (_upgradeModalInstance == null)
        {
            if (_upgradeModalPrefab == null)
            {
                Debug.LogError("[DayService] Upgrade modal prefab not loaded!");
                return;
            }
            _upgradeModalInstance = Instantiate(_upgradeModalPrefab, canvas.transform);
        }

        _upgradeModalInstance.SetActive(true);
        UpgradeModalController modal = _upgradeModalInstance.GetComponent<UpgradeModalController>();
        if (modal != null)
        {
            modal.Setup(completedDay, _upgradeManager, () => _upgradeModalInstance.SetActive(false));
        }
    }

    public void ShowDayFailure(int day)
    {
        Debug.Log($"Day {day} failed - quota not met!");
    }

    public void ShowEnding()
    {
        Time.timeScale = 0f;

        if (_endingPanelInstance == null)
        {
            _endingPanelInstance = Instantiate(endingPanelPrefab, canvas.transform);
        }

        _endingPanelInstance.SetActive(true);
        EndingController ending = _endingPanelInstance.GetComponent<EndingController>();
        if (ending != null)
        {
            ending.PlayEnding();
        }
    }
}
