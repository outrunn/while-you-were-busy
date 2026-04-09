using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// One-click setup - creates all scene assets with correct positions from sprite assets.
/// Just call CreateAllAssets() and it handles everything.
/// </summary>
public class SetupAllAssets : MonoBehaviour
{
    [SerializeField] private Sprite bulletinBoardSprite;
    [SerializeField] private Sprite printerSprite;
    [SerializeField] private Sprite shredderSprite;
    [SerializeField] private Sprite flowerSprite;
    [SerializeField] private Sprite windowSprite;
    [SerializeField] private Sprite ticketSprite;
    [SerializeField] private Sprite stampSprite;
    [SerializeField] private Sprite wallpaperSprite;
    [SerializeField] private Sprite deskSprite;
    [SerializeField] private Sprite laptopSprite;
    [SerializeField] private GameObject typingMinigamePrefab;

    private Canvas targetCanvas;

    private void Start()
    {
        // Auto-create assets on scene start if they don't exist
        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas != null && targetCanvas.transform.childCount == 0)
        {
            CreateAllAssets();
        }
    }

    [ContextMenu("Create All Assets")]
    public void CreateAllAssets()
    {
        if (targetCanvas == null)
            targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }

        // Clear existing UI assets
        ClearExistingAssets();

        // Create background elements (should be behind everything else)
        CreateAsset("Wallpaper", wallpaperSprite, new Vector2(-1389f, -735f), new Vector2(1920f, 1080f));
        CreateAsset("Desk", deskSprite, new Vector2(-5f, -345f), new Vector2(1920f, 200f));

        // Create main scene assets
        CreateAsset("BulletinBoard", bulletinBoardSprite, new Vector2(-418f, 180f), new Vector2(785f, 398f));
        CreateAsset("Printer", printerSprite, new Vector2(-562f, -232f), new Vector2(296f, 367f));
        CreateAsset("Shredder", shredderSprite, new Vector2(576f, -215f), new Vector2(250f, 280f));
        CreateAsset("Flower", flowerSprite, new Vector2(-307f, -246f), new Vector2(94f, 210f));
        CreateAsset("Window", windowSprite, new Vector2(354f, 142f), new Vector2(541f, 455f));
        CreateAsset("Ticket", ticketSprite, new Vector2(-406f, 262f), new Vector2(140f, 77f));

        // Don't create stamp - it will be created dynamically when tickets are completed

        // Setup minigame UI with proper Canvas layering
        SetupMinigameUI();

        Debug.Log("✓ All assets created and positioned!");
    }

    /// <summary>
    /// Instantiate the typing minigame UI prefab as a child of the main canvas
    /// with Canvas component set to render on top (sortOrder=100)
    /// </summary>
    private void SetupMinigameUI()
    {
        if (typingMinigamePrefab == null)
        {
            Debug.LogWarning("TypingMinigamePrefab is not assigned in Inspector! Minigame UI may not render on top.");
            return;
        }

        // Check if already instantiated
        if (targetCanvas.transform.Find("TypingMinigameUI") != null)
        {
            Debug.Log("TypingMinigameUI already exists in scene");
            return;
        }

        GameObject minigameInstance = Instantiate(typingMinigamePrefab, targetCanvas.transform, false);
        minigameInstance.name = "TypingMinigameUI";

        // Ensure Canvas component has correct sort order
        Canvas minigameCanvas = minigameInstance.GetComponent<Canvas>();
        if (minigameCanvas != null)
        {
            minigameCanvas.sortingOrder = 100;
            Debug.Log("✓ TypingMinigameUI Canvas sortOrder set to 100");
        }
        else
        {
            Debug.LogWarning("TypingMinigameUI prefab missing Canvas component!");
        }
    }

    private GameObject CreateAsset(string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"Sprite for {name} is not assigned! Make sure to assign sprites in the Inspector.");
            return null;
        }

        GameObject obj = new GameObject(name);
        obj.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        // Attach ShredderUI script if this is the shredder
        if (name == "Shredder")
        {
            ShredderUI shredderUI = obj.AddComponent<ShredderUI>();
            obj.AddComponent<GraphicRaycaster>();
        }

        Debug.Log($"Created asset: {name}");
        return obj;
    }

    private void ClearExistingAssets()
    {
        List<Transform> toDelete = new List<Transform>();
        foreach (Transform child in targetCanvas.transform)
        {
            if (child.name.Contains("Board") || child.name.Contains("Printer") ||
                child.name.Contains("Shredder") || child.name.Contains("Flower") ||
                child.name.Contains("Window") || child.name.Contains("Ticket") ||
                child.name.Contains("Stamp") || child.name.Contains("Wallpaper") ||
                child.name.Contains("Desk"))
            {
                toDelete.Add(child);
            }
        }

        foreach (Transform t in toDelete)
            Destroy(t.gameObject);
    }
}
