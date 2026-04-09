using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Math minigame: player solves arithmetic problems (addition/subtraction) by typing the answer.
/// Keyboard-only input. Wrong answer flashes red and clears. Enter to submit.
/// </summary>
public class MathMinigameUI : MonoBehaviour
{
    public static MathMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameWindow;
    [SerializeField] private TextMeshProUGUI problemText;
    [SerializeField] private TMP_InputField inputField; // TMP Input Field for player input
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image windowBackground;

    [Header("Settings")]
    [SerializeField] private float completionDelay = 2.5f;
    [SerializeField] private float wrongAnswerFlashDuration = 0.3f;

    private bool isActive = false;
    private bool isCompleted = false;
    private System.Action onMinigameCompleted;

    private int firstOperand;
    private int secondOperand;
    private char operation; // '+' or '-'
    private int correctAnswer;

    private void Awake()
    {
        Debug.Log("MathMinigameUI.Awake called");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("MathMinigameUI Instance set");
        }
        else
        {
            Debug.Log("MathMinigameUI Instance already exists, destroying this one");
            Destroy(gameObject);
        }

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isActive || isCompleted) return;

        // With TMP Input Field, we don't need to manually handle input
        // The field handles it automatically. Just check for Enter to submit.
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitAnswer();
        }
    }

    /// <summary>
    /// Start the minigame
    /// </summary>
    public void StartMinigame(System.Action completionCallback)
    {
        Debug.Log("MathMinigameUI.StartMinigame called");
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;

        // Generate random problem based on difficulty
        GenerateProblem();
        Debug.Log($"Problem: {firstOperand} {operation} {secondOperand}, inputField: {inputField}");

        // Display problem
        if (problemText != null)
        {
            problemText.text = $"{firstOperand} {operation} {secondOperand} = ?";
            Debug.Log($"Set problem text");
        }
        else
        {
            Debug.LogError("problemText is NULL!");
        }

        // Clear input field
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField(); // Focus on input field
            Debug.Log($"Activated input field");
        }
        else
        {
            Debug.LogError("inputField is NULL!");
        }

        // Clear feedback
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        // Show window
        if (minigameWindow != null)
        {
            minigameWindow.SetActive(true);
        }
    }

    /// <summary>
    /// Generate a random math problem
    /// </summary>
    private void GenerateProblem()
    {
        // Random difficulty for variety
        int difficultyLevel = Random.Range(1, 4);

        // Determine operand ranges based on difficulty
        int minOperand, maxOperand;
        if (difficultyLevel == 1)
        {
            minOperand = 1;
            maxOperand = 20;
        }
        else if (difficultyLevel == 2)
        {
            minOperand = 10;
            maxOperand = 50;
        }
        else
        {
            minOperand = 20;
            maxOperand = 99;
        }

        firstOperand = Random.Range(minOperand, maxOperand + 1);
        secondOperand = Random.Range(minOperand, maxOperand + 1);

        // Randomly choose addition or subtraction
        operation = Random.value > 0.5f ? '+' : '-';

        // Calculate correct answer
        if (operation == '+')
        {
            correctAnswer = firstOperand + secondOperand;
        }
        else
        {
            // Ensure no negative results
            if (firstOperand < secondOperand)
            {
                int temp = firstOperand;
                firstOperand = secondOperand;
                secondOperand = temp;
            }
            correctAnswer = firstOperand - secondOperand;
        }
    }

    /// <summary>
    /// Submit the player's answer
    /// </summary>
    private void SubmitAnswer()
    {
        // Get answer from input field
        string answer = inputField != null ? inputField.text : "";
        if (answer.Length == 0) return;

        if (int.TryParse(answer, out int playerAnswer))
        {
            if (playerAnswer == correctAnswer)
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
                // Wrong answer
                if (feedbackText != null)
                {
                    feedbackText.text = "Incorrect! Try again.";
                    feedbackText.color = Color.red;
                }

                // Flash the background red
                if (windowBackground != null)
                {
                    StartCoroutine(FlashRed());
                }

                // Clear input field
                if (inputField != null)
                {
                    inputField.text = "";
                    inputField.ActivateInputField();
                }
            }
        }
    }

    /// <summary>
    /// Coroutine to flash background red on wrong answer
    /// </summary>
    private IEnumerator FlashRed()
    {
        Color originalColor = windowBackground.color;
        windowBackground.color = Color.red;
        yield return new WaitForSeconds(wrongAnswerFlashDuration);
        windowBackground.color = originalColor;
    }

    /// <summary>
    /// Complete the minigame
    /// </summary>
    private void CompleteMinigame()
    {
        isCompleted = true;
        isActive = false;
        StartCoroutine(CloseAfterDelay(completionDelay));
    }

    /// <summary>
    /// Wait then call completion callback and close
    /// </summary>
    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        onMinigameCompleted?.Invoke();
        CloseMinigame();
    }

    /// <summary>
    /// Close the minigame window
    /// </summary>
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
}
