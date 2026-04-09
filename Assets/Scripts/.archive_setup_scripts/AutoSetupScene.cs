using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Auto-setup script - just attach to Canvas and it will create all assets.
/// </summary>
public class AutoSetupScene : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            SetupAllAssets(canvas);
        }
    }

    private void SetupAllAssets(Canvas canvas)
    {
        // Load sprites from Assets/UI_Assets
        Sprite wallpaper = Resources.Load<Sprite>("UI_Assets/image 2");
        Sprite desk = Resources.Load<Sprite>("UI_Assets/image 5");
        Sprite bulletinBoard = Resources.Load<Sprite>("UI_Assets/Bulletin Board");
        Sprite printer = Resources.Load<Sprite>("UI_Assets/Printer");
        Sprite shredder = Resources.Load<Sprite>("UI_Assets/Shredder");
        Sprite flower = Resources.Load<Sprite>("UI_Assets/Flower");
        Sprite window = Resources.Load<Sprite>("UI_Assets/Window");
        Sprite ticket = Resources.Load<Sprite>("UI_Assets/Ticket");
        Sprite stamp = Resources.Load<Sprite>("UI_Assets/Rectangle 45");

        // Create background
        if (wallpaper != null)
            CreateAsset("Wallpaper", canvas, wallpaper, new Vector2(-1389f, -735f), new Vector2(1920f, 1080f));

        if (desk != null)
            CreateAsset("Desk", canvas, desk, new Vector2(-5f, -345f), new Vector2(1920f, 200f));

        // Create main assets
        if (bulletinBoard != null)
            CreateAsset("BulletinBoard", canvas, bulletinBoard, new Vector2(-418f, 180f), new Vector2(785f, 398f));

        if (printer != null)
            CreateAsset("Printer", canvas, printer, new Vector2(-562f, -232f), new Vector2(296f, 367f));

        if (shredder != null)
            CreateAsset("Shredder", canvas, shredder, new Vector2(576f, -215f), new Vector2(250f, 280f));

        if (flower != null)
            CreateAsset("Flower", canvas, flower, new Vector2(-307f, -246f), new Vector2(94f, 210f));

        if (window != null)
            CreateAsset("Window", canvas, window, new Vector2(354f, 142f), new Vector2(541f, 455f));

        if (ticket != null)
            CreateAsset("Ticket", canvas, ticket, new Vector2(-406f, 262f), new Vector2(140f, 77f));

        // Don't create stamp - it will be created dynamically when tickets are completed

        Debug.Log("✓ Scene auto-setup complete!");
    }

    private GameObject CreateAsset(string name, Canvas canvas, Sprite sprite, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;

        return obj;
    }
}
