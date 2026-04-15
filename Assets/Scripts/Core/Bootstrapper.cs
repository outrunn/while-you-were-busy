using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private Transform minigameContainer;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        Debug.Log("[Bootstrapper] Starting initialization...");

        // Setup dev mode
        DevMode devMode = FindFirstObjectByType<DevMode>();
        if (devMode == null)
        {
            GameObject devObj = new GameObject("DevMode");
            devMode = devObj.AddComponent<DevMode>();
        }

        // Setup event system first
        GameEvents events = GetComponentInChildren<GameEvents>();
        if (events == null)
        {
            GameObject eventsObj = new GameObject("GameEvents");
            eventsObj.transform.SetParent(transform);
            events = eventsObj.AddComponent<GameEvents>();
        }

        // Find or create canvas
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[Bootstrapper] No Canvas found in scene!");
                return;
            }
        }

        // Find or create minigame container
        if (minigameContainer == null)
        {
            Transform container = canvas.transform.Find("MinigameContainer");
            if (container != null)
            {
                minigameContainer = container;
            }
            else
            {
                GameObject containerObj = new GameObject("MinigameContainer");
                containerObj.transform.SetParent(canvas.transform);
                minigameContainer = containerObj.transform;
            }
        }

        // Initialize services
        GameState gameState = new GameState();

        TypingTaskDatabase typingTaskDb = FindFirstObjectByType<TypingTaskDatabase>();
        if (typingTaskDb == null)
        {
            Debug.LogWarning("[Bootstrapper] TypingTaskDatabase not found. Creating one.");
            GameObject db = new GameObject("TypingTaskDatabase");
            typingTaskDb = db.AddComponent<TypingTaskDatabase>();
        }

        TaskService taskService = new TaskService(typingTaskDb);
        TicketService ticketService = new TicketService(Constants.MAX_ACTIVE_TICKETS);
        MinigameFactory minigameFactory = new MinigameFactory(minigameContainer);

        // Get or create DayService
        DayService dayService = GetComponentInChildren<DayService>();
        if (dayService == null)
        {
            GameObject dayObj = new GameObject("DayService");
            dayObj.transform.SetParent(transform);
            dayService = dayObj.AddComponent<DayService>();
        }

        // Get or create UpgradeManager
        UpgradeManager upgradeManager = GetComponentInChildren<UpgradeManager>();
        if (upgradeManager == null)
        {
            GameObject upgradeObj = new GameObject("UpgradeManager");
            upgradeObj.transform.SetParent(transform);
            upgradeManager = upgradeObj.AddComponent<UpgradeManager>();
        }

        // Get or create GameService
        GameService gameService = GetComponentInChildren<GameService>();
        if (gameService == null)
        {
            GameObject gameObj = new GameObject("GameService");
            gameObj.transform.SetParent(transform);
            gameService = gameObj.AddComponent<GameService>();
        }

        // Initialize GameService with all dependencies
        gameService.Initialize(gameState, taskService, ticketService, minigameFactory, dayService, upgradeManager);

        // Setup Printer references
        if (canvas.GetComponent<PrinterSetup>() == null)
            canvas.gameObject.AddComponent<PrinterSetup>();

        // Initialize Controllers
        InitializeControllers(canvas, gameState, taskService, ticketService, minigameFactory);

        Debug.Log("[Bootstrapper] ✓ All systems initialized");
    }

    private void InitializeControllers(Canvas canvas, GameState gameState, TaskService taskService,
                                      TicketService ticketService, MinigameFactory minigameFactory)
    {
        // Initialize GameHUDController
        GameHUDController hudController = canvas.GetComponentInChildren<GameHUDController>();
        if (hudController == null)
        {
            Transform hudPanel = canvas.transform.Find("GameHUD");
            if (hudPanel != null)
            {
                hudController = hudPanel.gameObject.AddComponent<GameHUDController>();
            }
        }

        if (hudController != null)
        {
            hudController.Initialize(gameState);
        }

        // Initialize PrinterController
        PrinterController printerController = canvas.GetComponentInChildren<PrinterController>();
        Debug.Log($"[Bootstrapper] Found PrinterController: {printerController != null}");

        if (printerController != null)
        {
            // Get GameService from the gameobject we created earlier
            GameService gameService = GetComponentInChildren<GameService>();
            Debug.Log($"[Bootstrapper] Found GameService: {gameService != null}");

            if (gameService != null)
            {
                Debug.Log("[Bootstrapper] Calling printerController.Initialize()");
                printerController.Initialize(gameService, minigameFactory);
            }
        }
        else
        {
            Debug.LogWarning("[Bootstrapper] PrinterController not found in Canvas children!");
        }
    }
}
