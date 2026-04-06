using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Data Entry minigame: player must reproduce a number sequence by clicking digit buttons.
/// </summary>
public class DataEntryMinigameUI : MonoBehaviour
{
    public static DataEntryMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameWindow;
    [SerializeField] private TextMeshProUGUI targetNumberText;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Transform digitButtonContainer;

    [Header("Settings")]
    [SerializeField] private float completionDelay = 2.5f;

    private string targetNumber = "";
    private string playerInput = "";
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

    public void StartMinigame(string target, System.Action completionCallback)
    {
        targetNumber = target;
        playerInput = "";
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;

        if (targetNumberText != null)
        {
            targetNumberText.text = targetNumber;
        }

        if (inputText != null)
        {
            inputText.text = "";
        }

        if (hintText != null)
        {
            hintText.text = "";
        }

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }
    }

    public void OnDigitClicked(int digit)
    {
        if (!isActive || isCompleted) return;

        playerInput += digit.ToString();

        if (inputText != null)
        {
            inputText.text = playerInput;
        }

        // Check if input matches so far
        if (playerInput.Length <= targetNumber.Length && targetNumber.StartsWith(playerInput))
        {
            // Still matching
            if (inputText != null)
            {
                inputText.color = Color.white;
            }

            // Check if complete
            if (playerInput == targetNumber)
            {
                CompleteMinigame();
            }
        }
        else
        {
            // Wrong input
            StartCoroutine(ErrorFlash());
        }
    }

    private IEnumerator ErrorFlash()
    {
        if (inputText != null)
        {
            inputText.color = Color.red;
            yield return new WaitForSeconds(0.3f);

            // Show hint if Number Lock upgrade is active
            if (UpgradeManager.Instance != null && UpgradeManager.Instance.IsNumberLockActive())
            {
                if (hintText != null)
                {
                    hintText.text = $"Answer: {targetNumber} (revealed)";
                    hintText.color = new Color(1, 1, 1, 0.4f);
                }
            }

            inputText.color = Color.white;
            playerInput = "";
            if (inputText != null)
            {
                inputText.text = "";
            }
        }
    }

    private void CompleteMinigame()
    {
        isCompleted = true;
        isActive = false;

        if (hintText != null)
        {
            hintText.text = "Complete!";
            hintText.color = Color.green;
        }

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
}
