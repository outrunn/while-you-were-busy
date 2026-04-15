using System.Collections;
using UnityEngine;
using TMPro;

public class DayService : MonoBehaviour
{
    [SerializeField] private GameObject upgradeModalPrefab;
    [SerializeField] private GameObject endingPanelPrefab;
    [SerializeField] private Canvas canvas;

    private GameObject _upgradeModalInstance;
    private GameObject _endingPanelInstance;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
    }

    public void ShowUpgradeModal(int completedDay)
    {
        if (_upgradeModalInstance == null)
        {
            _upgradeModalInstance = Instantiate(upgradeModalPrefab, canvas.transform);
        }

        _upgradeModalInstance.SetActive(true);
        UpgradeModalController modal = _upgradeModalInstance.GetComponent<UpgradeModalController>();
        if (modal != null)
        {
            modal.Setup(completedDay, () => _upgradeModalInstance.SetActive(false));
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
