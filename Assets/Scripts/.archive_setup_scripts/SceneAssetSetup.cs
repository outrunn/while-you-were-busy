using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets up all scene UI assets (bulletin board, flower, printer, shredder, ticket, approved stamp, window)
/// from imported sprites. Can be used to initialize or refresh the scene layout.
/// </summary>
public class SceneAssetSetup : MonoBehaviour
{
    [Header("Asset Prefab References")]
    [SerializeField] private Sprite bulletinBoardSprite;
    [SerializeField] private Sprite flowerSprite;
    [SerializeField] private Sprite printerSprite;
    [SerializeField] private Sprite shredderSprite;
    [SerializeField] private Sprite ticketSprite;
    [SerializeField] private Sprite approvedStampSprite;
    [SerializeField] private Sprite windowSprite;

    [Header("Layout Settings")]
    [SerializeField] private Canvas sceneCanvas;
    [SerializeField] private Vector2 bulletinBoardPosition = new Vector2(-418f, 180f);
    [SerializeField] private Vector2 bulletinBoardSize = new Vector2(785f, 398f);

    [SerializeField] private Vector2 flowerPosition = new Vector2(-307f, -246f);
    [SerializeField] private Vector2 flowerSize = new Vector2(94f, 210f);

    [SerializeField] private Vector2 printerPosition = new Vector2(-562f, -232f);
    [SerializeField] private Vector2 printerSize = new Vector2(296f, 367f);

    [SerializeField] private Vector2 shredderPosition = new Vector2(576f, -215f);
    [SerializeField] private Vector2 shredderSize = new Vector2(250f, 280f);

    [SerializeField] private Vector2 ticketPosition = new Vector2(-406f, 262f);
    [SerializeField] private Vector2 ticketSize = new Vector2(140f, 77f);

    [SerializeField] private Vector2 windowPosition = new Vector2(354f, 142f);
    [SerializeField] private Vector2 windowSize = new Vector2(541f, 455f);

    private void Start()
    {
        if (sceneCanvas == null)
        {
            sceneCanvas = FindFirstObjectByType<Canvas>();
        }

        InitializeSceneAssets();
    }

    /// <summary>
    /// Initialize all scene assets if they don't already exist
    /// </summary>
    public void InitializeSceneAssets()
    {
        if (sceneCanvas == null)
        {
            Debug.LogError("SceneAssetSetup: No Canvas found in scene!");
            return;
        }

        // Create each asset if sprites are assigned
        if (bulletinBoardSprite != null)
            CreateAssetUI("BulletinBoardUI", bulletinBoardSprite, bulletinBoardPosition, bulletinBoardSize);

        if (flowerSprite != null)
            CreateAssetUI("FlowerUI", flowerSprite, flowerPosition, flowerSize);

        if (printerSprite != null)
            CreateAssetUI("PrinterUI", printerSprite, printerPosition, printerSize);

        if (shredderSprite != null)
            CreateAssetUI("ShredderUI", shredderSprite, shredderPosition, shredderSize);

        if (ticketSprite != null)
            CreateAssetUI("TicketUI", ticketSprite, ticketPosition, ticketSize);

        // Stamp is created dynamically when tickets are completed, not in the scene

        if (windowSprite != null)
            CreateAssetUI("WindowUI", windowSprite, windowPosition, windowSize);
    }

    /// <summary>
    /// Create a UI element from a sprite
    /// </summary>
    private GameObject CreateAssetUI(string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        // Check if already exists
        if (sceneCanvas.transform.Find(name) != null)
        {
            Debug.LogWarning($"SceneAssetSetup: {name} already exists in scene");
            return null;
        }

        GameObject assetObject = new GameObject(name);
        assetObject.transform.SetParent(sceneCanvas.transform, false);

        RectTransform rectTransform = assetObject.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        Image image = assetObject.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        // Attach ShredderUI script if this is the shredder
        if (name == "ShredderUI")
        {
            ShredderUI shredderUI = assetObject.AddComponent<ShredderUI>();
            assetObject.AddComponent<GraphicRaycaster>();
        }

        return assetObject;
    }

    /// <summary>
    /// Refresh all assets (useful if sprites are reassigned)
    /// </summary>
    public void RefreshAssets()
    {
        // Clear existing assets
        foreach (Transform child in sceneCanvas.transform)
        {
            if (child.name.EndsWith("UI"))
            {
                Destroy(child.gameObject);
            }
        }

        // Reinitialize
        InitializeSceneAssets();
    }
}
