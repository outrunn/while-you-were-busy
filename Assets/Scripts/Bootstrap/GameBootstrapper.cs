using UnityEngine;

/// <summary>
/// Single entry point for the game. Initializes all systems in correct order.
/// Add this to GameScene as a component on any GameObject (e.g., a "Bootstrapper" empty object).
///
/// Initialization Order:
/// 1. GameManager (core game state, day timer)
/// 2. Printer (spawns tickets on schedule)
/// 3. BulletinBoard (manages on-screen tickets)
/// 4. MinigameManager (prevents minigame overlap)
///
/// This ensures dependencies are satisfied in the correct order.
/// </summary>
public class GameBootstrapper : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Printer printer;
    [SerializeField] private BulletinBoard bulletinBoard;
    [SerializeField] private MinigameManager minigameManager;

    [Header("Minigame Prefabs")]
    [SerializeField] private TypingMinigameUI typingMinigamePrefab;
    [SerializeField] private MathMinigameUI mathMinigamePrefab;
    [SerializeField] private MultipleChoiceMinigameUI multipleChoiceMinigamePrefab;
    [SerializeField] private PhotoRevealMinigameUI photoRevealMinigamePrefab;

    [Header("UI Prefabs")]
    [SerializeField] private Canvas canvasPrefab;
    [SerializeField] private Transform minigameContainer;

    private void Awake()
    {
        Debug.Log("[GameBootstrapper] Initializing game systems...");

        // Step 1: Initialize GameManager (must be first - core state)
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        if (gameManager == null)
        {
            Debug.LogError("[GameBootstrapper] GameManager not found in scene! Cannot initialize game.");
            return;
        }
        Debug.Log("[GameBootstrapper] ✓ GameManager ready");

        // Step 2: Initialize Printer (depends on GameManager for day info)
        if (printer == null)
        {
            printer = FindFirstObjectByType<Printer>();
        }
        if (printer == null)
        {
            Debug.LogError("[GameBootstrapper] Printer not found in scene!");
        }
        else
        {
            Debug.Log("[GameBootstrapper] ✓ Printer ready");
        }

        // Step 3: Initialize BulletinBoard (depends on Printer for tickets)
        if (bulletinBoard == null)
        {
            bulletinBoard = FindFirstObjectByType<BulletinBoard>();
        }
        if (bulletinBoard == null)
        {
            Debug.LogError("[GameBootstrapper] BulletinBoard not found in scene!");
        }
        else
        {
            Debug.Log("[GameBootstrapper] ✓ BulletinBoard ready");
        }

        // Step 4: Initialize MinigameManager (depends on GameManager for callbacks)
        if (minigameManager == null)
        {
            minigameManager = FindFirstObjectByType<MinigameManager>();
        }
        if (minigameManager == null)
        {
            Debug.LogWarning("[GameBootstrapper] MinigameManager not found. Ensure it exists in scene.");
        }
        else
        {
            Debug.Log("[GameBootstrapper] ✓ MinigameManager ready");
        }

        Debug.Log("[GameBootstrapper] ✓ All systems initialized. Game ready to start.");
    }

    private void Start()
    {
        // After all systems are initialized, set up UI assets from Resources
        Debug.Log("[GameBootstrapper] Setting up UI assets...");
        AutoSetupScene.SetupAllAssets();
    }
}
