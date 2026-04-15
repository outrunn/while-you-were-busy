using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Multiple Choice minigame: answer a work-themed question with 4 options.
/// One wrong answer triggers a red flash and input is reset. Correct answer completes immediately.
/// </summary>
public class MultipleChoiceMinigameUI : BaseMinigameUI
{
    public static MultipleChoiceMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image windowBackground;

    // Question/Answer data
    private struct QuestionData
    {
        public string question;
        public int correctAnswerIndex; // 0-3
        public string[] answers;
    }

    private QuestionData[] questions = new QuestionData[]
    {
        new QuestionData
        {
            question = "How many months have exactly 28 days?",
            correctAnswerIndex = 0,
            answers = new[] { "12", "1", "6", "0" }
        },
        new QuestionData
        {
            question = "What gets wetter the more it dries?",
            correctAnswerIndex = 0,
            answers = new[] { "A towel", "A sponge", "Water", "Soap" }
        },
        new QuestionData
        {
            question = "You're running a race and pass the person in 2nd place. What place are you now in?",
            correctAnswerIndex = 0,
            answers = new[] { "2nd", "1st", "3rd", "It depends" }
        },
        new QuestionData
        {
            question = "Which is heavier: a pound of feathers or a pound of bricks?",
            correctAnswerIndex = 0,
            answers = new[] { "They weigh the same", "Feathers", "Bricks", "Depends on the bricks" }
        },
        new QuestionData
        {
            question = "How many sides does a circle have?",
            correctAnswerIndex = 0,
            answers = new[] { "2", "0", "1", "Infinite" }
        },
        new QuestionData
        {
            question = "If you have 3 apples and take away 2, how many do you have?",
            correctAnswerIndex = 0,
            answers = new[] { "2", "1", "3", "0" }
        },
        new QuestionData
        {
            question = "Before Mt. Everest was discovered, what was the tallest mountain?",
            correctAnswerIndex = 0,
            answers = new[] { "Everest", "K2", "Kilimanjaro", "No mountain" }
        },
        new QuestionData
        {
            question = "A plane crashes on the border of the U.S. and Canada. Where do they bury the survivors?",
            correctAnswerIndex = 0,
            answers = new[] { "Nowhere", "U.S.", "Canada", "Both" }
        },
        new QuestionData
        {
            question = "How many animals of each species did Moses take on the Ark?",
            correctAnswerIndex = 0,
            answers = new[] { "None", "2", "1", "7" }
        },
        new QuestionData
        {
            question = "If you drop a red shirt into the Red Sea, what happens?",
            correctAnswerIndex = 0,
            answers = new[] { "It gets wet", "It sinks", "It disappears", "It gets dyed" }
        }
    };

    private QuestionData currentQuestion;

    private void Awake()
    {
        Debug.Log("MultipleChoiceMinigameUI.Awake called");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("MultipleChoiceMinigameUI Instance set");
        }
        else
        {
            Debug.Log("MultipleChoiceMinigameUI Instance already exists, destroying this one");
            Destroy(gameObject);
        }

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    private void Start()
    {
        Debug.Log("MultipleChoiceMinigameUI.Start called");
        // Wire up all answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null)
            {
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
                Debug.Log($"Wired up button {i}");
            }
            else
            {
                Debug.LogError($"answerButtons[{i}] is NULL!");
            }
        }
    }


    /// <summary>
    /// Called when player clicks an answer button
    /// </summary>
    private void OnAnswerClicked(int answerIndex)
    {
        Debug.Log($"OnAnswerClicked: {answerIndex}, isActive={isActive}, isCompleted={isCompleted}");
        if (!isActive || isCompleted) return;

        Debug.Log($"Checking answer {answerIndex} vs correct {currentQuestion.correctAnswerIndex}");
        if (answerIndex == currentQuestion.correctAnswerIndex)
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
                StartCoroutine(FlashRed(windowBackground));
            }
        }
    }

    /// <summary>
    /// Override StartMinigame to initialize answer buttons
    /// </summary>
    public override void StartMinigame(System.Action completionCallback)
    {
        Debug.Log("MultipleChoiceMinigameUI.StartMinigame called");
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;

        // Wire up buttons if not already done
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null)
            {
                answerButtons[i].onClick.RemoveAllListeners(); // Clear old listeners
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
                Debug.Log($"Wired up button {i} in StartMinigame");
            }
        }

        // Pick a random question
        currentQuestion = questions[Random.Range(0, questions.Length)];
        Debug.Log($"Question: {currentQuestion.question}, questionText component: {questionText}");

        // Display question and answers
        if (questionText != null)
        {
            questionText.text = currentQuestion.question;
            Debug.Log($"Set question text to: {currentQuestion.question}");
        }
        else
        {
            Debug.LogError("questionText is NULL!");
        }

        for (int i = 0; i < answerTexts.Length && i < currentQuestion.answers.Length; i++)
        {
            if (answerTexts[i] != null)
            {
                answerTexts[i].text = currentQuestion.answers[i];
                Debug.Log($"Set answer {i} to: {currentQuestion.answers[i]}");
            }
            else
            {
                Debug.LogError($"answerTexts[{i}] is NULL!");
            }
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
    /// Override CloseMinigame to ensure proper state reset
    /// </summary>
    public override void CloseMinigame()
    {
        base.CloseMinigame();

        // Clear button listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
}
