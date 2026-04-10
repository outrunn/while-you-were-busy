using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Math minigame: player solves arithmetic problems (addition/subtraction) by typing the answer.
/// Keyboard-only input. Wrong answer flashes red and clears. Enter to submit.
/// </summary>
public class MathMinigameUI : BaseMinigameUI
{
    public static MathMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI problemText;
    [SerializeField] private TMP_InputField inputField; // TMP Input Field for player input
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image windowBackground;

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
    /// Override StartMinigame to initialize the math problem
    /// </summary>
    public override void StartMinigame(System.Action completionCallback)
    {
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;

        // Generate random problem based on difficulty
        GenerateProblem();

        // Display problem
        if (problemText != null)
        {
            problemText.text = $"{firstOperand} {operation} {secondOperand} = ?";
        }

        // Clear input field
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }

        // Clear feedback
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        // Show window
        ShowWindow();
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

                CompleteMinigameInternal();
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
                    StartCoroutine(FlashRed(windowBackground));
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
    /// Override CompleteMinigame to use base class implementation
    /// </summary>
    private void CompleteMinigameInternal()
    {
        CompleteMinigame(); // Call base class method
    }

    /// <summary>
    /// Override CloseMinigame to reset game state
    /// </summary>
    public override void CloseMinigame()
    {
        base.CloseMinigame();

        // Reset input field
        if (inputField != null)
        {
            inputField.text = "";
        }
    }
}
