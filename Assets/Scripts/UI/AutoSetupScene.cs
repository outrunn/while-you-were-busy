using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto-setup script - loads all UI assets from Resources and creates them in the scene.
/// Properly configures Canvas and UI elements for screen-space rendering.
/// </summary>
public class AutoSetupScene : MonoBehaviour
{
    public static void SetupAllAssets()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[AutoSetupScene] No Canvas found in scene!");
            return;
        }

        // Clean up any test UI elements
        Transform testImage = canvas.transform.Find("TestImage");
        if (testImage != null)
        {
            DestroyImmediate(testImage.gameObject);
        }

        // Configure Canvas for proper rendering
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        // Ensure GraphicRaycaster exists for input
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        // Load sprites from Resources/UI_Assets
        Sprite wallpaper = Resources.Load<Sprite>("UI_Assets/image 2");
        Sprite desk = Resources.Load<Sprite>("UI_Assets/image 5");
        Sprite bulletinBoard = Resources.Load<Sprite>("UI_Assets/Bulletin Board");
        Sprite printer = Resources.Load<Sprite>("UI_Assets/Printer");
        Sprite shredder = Resources.Load<Sprite>("UI_Assets/Shredder");
        Sprite flower = Resources.Load<Sprite>("UI_Assets/Flower");
        Sprite window = Resources.Load<Sprite>("UI_Assets/Window");
        Sprite stamp = Resources.Load<Sprite>("UI_Assets/Rectangle 45");
        Sprite laptop = Resources.Load<Sprite>("UI_Assets/Laptop");
        Sprite frame = Resources.Load<Sprite>("UI_Assets/Frame 1");

        // Layer 0: Background wallpaper (behind everything)
        if (wallpaper != null)
            CreateAsset("Wallpaper", canvas, wallpaper, new Vector2(960f, 540f), new Vector2(1920f, 1080f), 0, false);

        // Layer 1: Desk at bottom
        if (desk != null)
            CreateAsset("Desk", canvas, desk, new Vector2(960f, 70f), new Vector2(1920f, 200f), 1, false);

        // Layer 5: Main gameplay elements
        if (bulletinBoard != null)
            CreateAsset("BulletinBoard", canvas, bulletinBoard, new Vector2(540f, 400f), new Vector2(785f, 398f), 5, false);

        if (printer != null)
            CreateAsset("Printer", canvas, printer, new Vector2(200f, 150f), new Vector2(296f, 367f), 5, false);

        if (shredder != null)
            CreateAsset("Shredder", canvas, shredder, new Vector2(1720f, 150f), new Vector2(250f, 280f), 5, false);

        if (flower != null)
            CreateAsset("Flower", canvas, flower, new Vector2(350f, 140f), new Vector2(94f, 210f), 5, false);

        if (window != null)
            CreateAsset("Window", canvas, window, new Vector2(1520f, 380f), new Vector2(541f, 455f), 5, false);

        if (laptop != null)
            CreateAsset("Laptop", canvas, laptop, new Vector2(960f, 450f), new Vector2(900f, 600f), 4, false);

        if (frame != null)
            CreateAsset("Frame", canvas, frame, new Vector2(960f, 450f), new Vector2(800f, 600f), 3, false);

        Debug.Log("[AutoSetupScene] ✓ All UI assets loaded and positioned!");
    }

    private static GameObject CreateAsset(string name, Canvas canvas, Sprite sprite, Vector2 position, Vector2 size, int sortingOrder, bool raycastTarget)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = raycastTarget;

        return obj;
    }
}
