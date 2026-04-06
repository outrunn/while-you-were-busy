using UnityEngine;

/// <summary>
/// Central setup script that instantiates all required game systems from prefabs.
/// Drop this on an empty GameObject to initialize the entire game.
/// </summary>
public class GameSetup : MonoBehaviour
{
    [Header("System Prefabs")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject dayManagerPrefab;
    [SerializeField] private GameObject upgradeManagerPrefab;
    [SerializeField] private GameObject systemLogPrefab;
    [SerializeField] private GameObject taskDatabasePrefab;
    [SerializeField] private GameObject typingTaskDatabasePrefab;
    [SerializeField] private GameObject backgroundControllerPrefab;
    [SerializeField] private GameObject typingMinigamePrefab;
    [SerializeField] private GameObject dataEntryMinigamePrefab;
    [SerializeField] private GameObject fileSortingMinigamePrefab;
    [SerializeField] private GameObject formReviewMinigamePrefab;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject upgradeModalPrefab;
    [SerializeField] private GameObject endingPanelPrefab;

    private void Awake()
    {
        InstantiateAllSystems();
    }

    private void InstantiateAllSystems()
    {
        // Instantiate all required singletons and UI systems
        if (gameManagerPrefab != null) Instantiate(gameManagerPrefab);
        if (dayManagerPrefab != null) Instantiate(dayManagerPrefab);
        if (upgradeManagerPrefab != null) Instantiate(upgradeManagerPrefab);
        if (systemLogPrefab != null) Instantiate(systemLogPrefab);
        if (taskDatabasePrefab != null) Instantiate(taskDatabasePrefab);
        if (typingTaskDatabasePrefab != null) Instantiate(typingTaskDatabasePrefab);
        if (backgroundControllerPrefab != null) Instantiate(backgroundControllerPrefab);
        if (typingMinigamePrefab != null) Instantiate(typingMinigamePrefab);
        if (dataEntryMinigamePrefab != null) Instantiate(dataEntryMinigamePrefab);
        if (fileSortingMinigamePrefab != null) Instantiate(fileSortingMinigamePrefab);
        if (formReviewMinigamePrefab != null) Instantiate(formReviewMinigamePrefab);

        // Store modal prefabs for DayManager to use
        if (upgradeModalPrefab != null && DayManager.Instance != null)
        {
            // DayManager will instantiate these on demand
        }

        if (endingPanelPrefab != null && DayManager.Instance != null)
        {
            // DayManager will instantiate this on Day 5 ending
        }

        Debug.Log("Game systems initialized!");
    }
}
