using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loads all UI assets from Resources/UI_Assets and creates them in the scene.
/// Runs after GameBootstrapper initializes core systems.
/// </summary>
public class UISetup : MonoBehaviour
{
    private Canvas canvas;

    private void Start()
    {
        // Find Canvas in scene
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[UISetup] No Canvas found in scene!");
            return;
        }

        SetupAllAssets();
    }

    private void SetupAllAssets()
    {
        // Load all sprites from Resources/UI_Assets
        Sprite wallpaper = Resources.Load<Sprite>("UI_Assets/image 2");
        Sprite desk = Resources.Load<Sprite>("UI_Assets/image 5");
        Sprite bulletinBoard = Resources.Load<Sprite>("UI_Assets/Bulletin Board");
        Sprite printer = Resources.Load<Sprite>("UI_Assets/Printer");
        Sprite shredder = Resources.Load<Sprite>("UI_Assets/Shredder");
        Sprite flower = Resources.Load<Sprite>("UI_Assets/Flower");
        Sprite window = Resources.Load<Sprite>("UI_Assets/Window");
        Sprite ticket = Resources.Load<Sprite>("UI_Assets/Ticket");
        Sprite stamp = Resources.Load<Sprite>("UI_Assets/Rectangle 45");
        Sprite laptop = Resources.Load<Sprite>("UI_Assets/Laptop");
        Sprite frame = Resources.Load<Sprite>("UI_Assets/Frame 1");

        // Create background - largest, renders first
        if (wallpaper != null)
            CreateAsset("Wallpaper", wallpaper, new Vector2(-1389f, -735f), new Vector2(1920f, 1080f), 0);

        if (desk != null)
            CreateAsset("Desk", desk, new Vector2(-5f, -345f), new Vector2(1920f, 200f), 1);

        // Create main assets
        if (bulletinBoard != null)
            CreateAsset("BulletinBoard", bulletinBoard, new Vector2(-418f, 180f), new Vector2(785f, 398f), 5);

        if (printer != null)
            CreateAsset("Printer", printer, new Vector2(-562f, -232f), new Vector2(296f, 367f), 5);

        if (shredder != null)
            CreateAsset("Shredder", shredder, new Vector2(576f, -215f), new Vector2(250f, 280f), 5);

        if (flower != null)
            CreateAsset("Flower", flower, new Vector2(-307f, -246f), new Vector2(94f, 210f), 5);

        if (window != null)
            CreateAsset("Window", window, new Vector2(354f, 142f), new Vector2(541f, 455f), 5);

        if (laptop != null)
            CreateAsset("Laptop", laptop, new Vector2(0f, 0f), new Vector2(900f, 600f), 10);

        if (frame != null)
            CreateAsset("Frame", frame, new Vector2(0f, 0f), new Vector2(800f, 600f), 5);

        Debug.Log("[UISetup] ✓ All UI assets created successfully!");
    }

    private GameObject CreateAsset(string name, Sprite sprite, Vector2 position, Vector2 size, int sortingOrder)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false; // Don't block raycast for background elements

        CanvasRenderer canvasRenderer = obj.GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            canvasRenderer.SetMaterial(image.material, 0);
        }

        return obj;
    }
}
