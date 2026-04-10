using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto-setup script - loads all UI assets from Resources and creates them in the scene.
/// Positions elements to work at any resolution with proper layering.
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

        // Ensure Canvas is set to Screen Space - Overlay
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // IMPORTANT: For ScreenSpaceOverlay, we DON'T need CanvasScaler at all
        // ScreenSpaceOverlay automatically fills the screen. Remove CanvasScaler if it exists.
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            DestroyImmediate(scaler);
        }

        // Set Canvas RectTransform to fill the entire screen
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;

        // Ensure GraphicRaycaster exists
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        // Load sprites from Resources/UI_Assets
        // Try loading both with and without the _0 suffix (for multi-sprite compatibility)
        Sprite wallpaper = Resources.Load<Sprite>("UI_Assets/image 2") ?? Resources.Load<Sprite>("UI_Assets/image 2/image 2_0");
        Sprite desk = Resources.Load<Sprite>("UI_Assets/image 5") ?? Resources.Load<Sprite>("UI_Assets/image 5/image 5_0");
        Sprite bulletinBoard = Resources.Load<Sprite>("UI_Assets/Bulletin Board") ?? Resources.Load<Sprite>("UI_Assets/Bulletin Board/Bulletin Board_0");
        Sprite printer = Resources.Load<Sprite>("UI_Assets/Printer") ?? Resources.Load<Sprite>("UI_Assets/Printer/Printer_0");
        Sprite shredder = Resources.Load<Sprite>("UI_Assets/Shredder") ?? Resources.Load<Sprite>("UI_Assets/Shredder/Shredder_0");
        Sprite flower = Resources.Load<Sprite>("UI_Assets/Flower") ?? Resources.Load<Sprite>("UI_Assets/Flower/Flower_0");
        Sprite window = Resources.Load<Sprite>("UI_Assets/Window") ?? Resources.Load<Sprite>("UI_Assets/Window/Window_0");
        Sprite stamp = Resources.Load<Sprite>("UI_Assets/Rectangle 45") ?? Resources.Load<Sprite>("UI_Assets/Rectangle 45/Rectangle 45_0");
        Sprite laptop = Resources.Load<Sprite>("UI_Assets/Laptop") ?? Resources.Load<Sprite>("UI_Assets/Laptop/Laptop_0");
        Sprite frame = Resources.Load<Sprite>("UI_Assets/Frame 1") ?? Resources.Load<Sprite>("UI_Assets/Frame 1/Frame 1_0");

        Debug.Log($"[AutoSetupScene] Loaded sprites - wallpaper: {wallpaper != null}, desk: {desk != null}, bulletin: {bulletinBoard != null}, printer: {printer != null}, shredder: {shredder != null}, flower: {flower != null}, window: {window != null}, laptop: {laptop != null}, frame: {frame != null}");

        // Layer 0: Background wallpaper (behind everything)
        // Position: top-left anchored, starts at (0,0), fills screen
        if (wallpaper != null)
            CreateAsset("Wallpaper", canvas, wallpaper, new Vector2(960f, -540f), new Vector2(1920f, 1080f), 0, false);

        // Layer 1: Desk at bottom
        if (desk != null)
            CreateAsset("Desk", canvas, desk, new Vector2(960f, -1010f), new Vector2(1920f, 200f), 1, false);

        // Layer 5: Main gameplay elements
        if (bulletinBoard != null)
            CreateAsset("BulletinBoard", canvas, bulletinBoard, new Vector2(540f, -500f), new Vector2(785f, 398f), 5, false);

        if (printer != null)
            CreateAsset("Printer", canvas, printer, new Vector2(200f, -250f), new Vector2(296f, 367f), 5, false);

        if (shredder != null)
            CreateAsset("Shredder", canvas, shredder, new Vector2(1720f, -250f), new Vector2(250f, 280f), 5, false);

        if (flower != null)
            CreateAsset("Flower", canvas, flower, new Vector2(350f, -240f), new Vector2(94f, 210f), 5, false);

        if (window != null)
            CreateAsset("Window", canvas, window, new Vector2(1520f, -550f), new Vector2(541f, 455f), 5, false);

        if (laptop != null)
            CreateAsset("Laptop", canvas, laptop, new Vector2(960f, -600f), new Vector2(900f, 600f), 4, false);

        if (frame != null)
            CreateAsset("Frame", canvas, frame, new Vector2(960f, -600f), new Vector2(800f, 600f), 3, false);

        Debug.Log("[AutoSetupScene] ✓ All UI assets loaded and positioned!");
        Debug.Log($"[AutoSetupScene] Canvas RectTransform: pos={canvas.GetComponent<RectTransform>().anchoredPosition}, size={canvas.GetComponent<RectTransform>().sizeDelta}, scaler={scaler?.uiScaleMode}");
    }

    private static GameObject CreateAsset(string name, Canvas canvas, Sprite sprite, Vector2 position, Vector2 size, int sortingOrder, bool raycastTarget)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        // For ScreenSpaceOverlay with anchored UI, use simple positioning
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        // Use 0,0 (top-left) anchoring since ScreenSpaceOverlay works with screen coords
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = raycastTarget;

        return obj;
    }
}
