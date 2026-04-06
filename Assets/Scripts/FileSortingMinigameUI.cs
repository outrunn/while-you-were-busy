using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// File Sorting minigame: drag date-labeled files into chronological order.
/// </summary>
public class FileSortingMinigameUI : MonoBehaviour
{
    public static FileSortingMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameWindow;
    [SerializeField] private Transform fileCardsContainer;
    [SerializeField] private Transform targetTrayContainer;
    [SerializeField] private GameObject fileCardPrefab;

    [Header("Settings")]
    [SerializeField] private float completionDelay = 2.5f;

    private List<FileSortCard> sortedCards = new List<FileSortCard>();
    private int correctCount = 0;
    private bool isActive = false;
    private bool isCompleted = false;
    private System.Action onMinigameCompleted;

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

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    public void StartMinigame(System.Action completionCallback)
    {
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;
        correctCount = 0;

        SetupFileCards();

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }
    }

    private void SetupFileCards()
    {
        // 5 date cards in random order
        string[] dates = { "Jan 12", "Feb 28", "Mar 3", "Apr 15", "May 22" };
        var dateList = new List<string>(dates);

        for (int i = dateList.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            var temp = dateList[i];
            dateList[i] = dateList[rand];
            dateList[rand] = temp;
        }

        sortedCards.Clear();
        int correctIndex = 0;
        bool preSorted = UpgradeManager.Instance != null && UpgradeManager.Instance.IsPreSortedActive();
        int preSortCount = preSorted ? 2 : 0;

        foreach (string date in dateList)
        {
            // Optionally pre-sort 2 cards if upgrade is active
            bool shouldPrePlace = preSorted && correctIndex < preSortCount;
            CreateFileCard(date, correctIndex, shouldPrePlace);
            correctIndex++;
        }
    }

    private void CreateFileCard(string dateLabel, int correctPosition, bool prePlaced)
    {
        var card = new FileSortCard
        {
            dateLabel = dateLabel,
            correctPosition = correctPosition,
            prePlaced = prePlaced
        };
        sortedCards.Add(card);
    }

    public void OnFileCardPlaced(string dateLabel, int position)
    {
        if (!isActive || isCompleted) return;

        var card = sortedCards.Find(c => c.dateLabel == dateLabel);
        if (card != null && card.correctPosition == position)
        {
            correctCount++;
            if (correctCount == 5)
            {
                CompleteMinigame();
            }
        }
    }

    private void CompleteMinigame()
    {
        isCompleted = true;
        isActive = false;

        float delay = completionDelay;
        StartCoroutine(CloseAfterDelay(delay));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        onMinigameCompleted?.Invoke();
        CloseMinigame();
    }

    public void CloseMinigame()
    {
        isActive = false;
        isCompleted = false;
        onMinigameCompleted = null;
        sortedCards.Clear();

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    public bool IsActive() => isActive;

    [System.Serializable]
    private class FileSortCard
    {
        public string dateLabel;
        public int correctPosition;
        public bool prePlaced;
    }
}
