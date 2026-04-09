using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets up the Printer GameObject with all required components and references.
/// This creates:
/// - Printer GameObject with Printer script
/// - TicketSpawnPoint child transform
/// - Assigns ticket prefab and spawn point references
/// - Links to other required components (BulletinBoard, AudioSource, etc.)
/// </summary>
public class PrinterSetup : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private AudioClip printSoundClip;

    [ContextMenu("Setup Printer")]
    public void SetupPrinter()
    {
        if (mainCanvas == null)
            mainCanvas = FindFirstObjectByType<Canvas>();

        if (mainCanvas == null)
        {
            Debug.LogError("PrinterSetup: No Canvas found in scene!");
            return;
        }

        // Check if Printer already exists
        Transform existingPrinter = mainCanvas.transform.Find("Printer");
        if (existingPrinter != null)
        {
            Debug.LogWarning("Printer already exists in scene. Skipping setup.");
            return;
        }

        // Create root Printer GameObject
        GameObject printerGO = new GameObject("Printer");
        printerGO.transform.SetParent(mainCanvas.transform, false);

        // Add RectTransform (UI element)
        RectTransform printerRect = printerGO.AddComponent<RectTransform>();
        printerRect.anchoredPosition = new Vector2(-562f, -232f);
        printerRect.sizeDelta = new Vector2(296f, 367f);

        // Create TicketSpawnPoint child (this is where tickets spawn from)
        GameObject spawnPointGO = new GameObject("TicketSpawnPoint");
        spawnPointGO.transform.SetParent(printerGO.transform, false);
        RectTransform spawnPointRect = spawnPointGO.AddComponent<RectTransform>();
        // Position at the top center of the printer (where tickets would "print out from")
        spawnPointRect.anchoredPosition = new Vector2(0f, 150f);
        spawnPointRect.sizeDelta = Vector2.zero;

        // Add Printer script component
        Printer printerScript = printerGO.AddComponent<Printer>();

        // Load ticket prefab if not assigned
        if (ticketPrefab == null)
        {
            ticketPrefab = Resources.Load<GameObject>("Prefabs/TicketPrefab");
            if (ticketPrefab == null)
            {
                Debug.LogError("PrinterSetup: Could not find TicketPrefab in Resources/Prefabs/!");
                return;
            }
        }

        // Assign references via reflection since fields are private SerializeField
        var ticketPrefabField = typeof(Printer).GetField("ticketPrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var spawnPointField = typeof(Printer).GetField("ticketSpawnPoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (ticketPrefabField != null)
            ticketPrefabField.SetValue(printerScript, ticketPrefab);

        if (spawnPointField != null)
            spawnPointField.SetValue(printerScript, spawnPointRect);

        // Create and assign audio source
        AudioSource audioSource = printerGO.AddComponent<AudioSource>();
        if (printSoundClip != null)
        {
            audioSource.clip = printSoundClip;
        }

        var audioSourceField = typeof(Printer).GetField("printSound",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (audioSourceField != null)
            audioSourceField.SetValue(printerScript, audioSource);

        Debug.Log("✓ Printer setup complete!");
        Debug.Log("  - Created Printer GameObject");
        Debug.Log("  - Created TicketSpawnPoint child at position (0, 150)");
        Debug.Log("  - Assigned Printer script and references");
    }
}
