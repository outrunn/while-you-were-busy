using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Form Review minigame: player finds and clicks the one incorrect field.
/// </summary>
public class FormReviewMinigameUI : MonoBehaviour
{
    public static FormReviewMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameWindow;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Transform fieldButtonsContainer;

    [Header("Settings")]
    [SerializeField] private float completionDelay = 2.5f;

    private int incorrectFieldIndex = -1;
    private int attempts = 3;
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
        attempts = 3;

        incorrectFieldIndex = Random.Range(0, 6);

        if (feedbackText != null)
        {
            feedbackText.text = $"Attempts: {attempts}";
        }

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }
    }

    public void OnFieldClicked(int fieldIndex)
    {
        if (!isActive || isCompleted) return;

        if (fieldIndex == incorrectFieldIndex)
        {
            // Correct!
            if (feedbackText != null)
            {
                feedbackText.text = "Correct!";
                feedbackText.color = Color.green;
            }
            CompleteMinigame();
        }
        else
        {
            // Wrong
            attempts--;
            if (feedbackText != null)
            {
                feedbackText.color = Color.red;
                feedbackText.text = $"Wrong! Attempts: {attempts}";
            }

            if (attempts <= 0)
            {
                if (feedbackText != null)
                {
                    feedbackText.text = "Game Over";
                }
                isActive = false;
                StartCoroutine(CloseAfterDelay(completionDelay));
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

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    public bool IsActive() => isActive;
    public bool IsIncorrectFieldHighlighted(int fieldIndex) => UpgradeManager.Instance?.IsQuickScanActive() == true && fieldIndex == incorrectFieldIndex;
}
