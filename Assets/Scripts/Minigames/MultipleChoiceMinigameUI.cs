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
            question = "Which file format is read-only?",
            correctAnswerIndex = 2,
            answers = new[] { "DOCX", "XLSX", "PDF", "TXT" }
        },
        new QuestionData
        {
            question = "What is the default backup interval?",
            correctAnswerIndex = 0,
            answers = new[] { "Daily", "Weekly", "Monthly", "Quarterly" }
        },
        new QuestionData
        {
            question = "Which protocol is most secure?",
            correctAnswerIndex = 3,
            answers = new[] { "HTTP", "FTP", "SSH", "TLS" }
        },
        new QuestionData
        {
            question = "Maximum file size per upload?",
            correctAnswerIndex = 1,
            answers = new[] { "256 MB", "512 MB", "1 GB", "2 GB" }
        },
        new QuestionData
        {
            question = "How many attempts for authentication?",
            correctAnswerIndex = 2,
            answers = new[] { "2", "3", "5", "10" }
        },
        new QuestionData
        {
            question = "What is the default password expiry?",
            correctAnswerIndex = 0,
            answers = new[] { "90 days", "6 months", "1 year", "Never" }
        },
        new QuestionData
        {
            question = "Which is a required field?",
            correctAnswerIndex = 1,
            answers = new[] { "Phone", "Email", "Department", "Notes" }
        },
        new QuestionData
        {
            question = "What time is the system maintenance window?",
            correctAnswerIndex = 3,
            answers = new[] { "9 AM", "12 PM", "3 PM", "2 AM" }
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
