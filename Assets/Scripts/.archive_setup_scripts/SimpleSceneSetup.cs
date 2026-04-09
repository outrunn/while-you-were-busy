using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Simple scene setup - loads all sprites from UI_Assets folder and creates them automatically.
/// No manual sprite assignment needed.
/// </summary>
public class SimpleSceneSetup : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }

        // Load all sprites from UI_Assets
        Sprite wallpaper = Resources.Load<Sprite>("Wallpapers/UI_Assets/image 2");
        if (wallpaper == null)
            wallpaper = LoadSpriteFromPath("Assets/UI_Assets/image 2.png");

        Sprite desk = LoadSpriteFromPath("Assets/UI_Assets/image 5.png");
        Sprite bulletinBoard = LoadSpriteFromPath("Assets/UI_Assets/Bulletin Board.png");
        Sprite printer = LoadSpriteFromPath("Assets/UI_Assets/Printer.png");
        Sprite shredder = LoadSpriteFromPath("Assets/UI_Assets/Shredder.png");
        Sprite flower = LoadSpriteFromPath("Assets/UI_Assets/Flower.png");
        Sprite window = LoadSpriteFromPath("Assets/UI_Assets/Window.png");
        Sprite ticket = LoadSpriteFromPath("Assets/UI_Assets/Ticket.png");
        Sprite laptop = LoadSpriteFromPath("Assets/UI_Assets/Laptop.png");
        if (laptop == null)
            Debug.LogWarning("Laptop sprite failed to load from Resources/UI_Assets/Laptop.png");

        // Create background
        if (wallpaper != null)
            CreateAsset(canvas, "Wallpaper", wallpaper, new Vector2(-1389f, -735f), new Vector2(1920f, 1080f));

        if (desk != null)
            CreateAsset(canvas, "Desk", desk, new Vector2(-5f, -345f), new Vector2(1920f, 200f));

        // Create main assets
        if (bulletinBoard != null)
            CreateAsset(canvas, "BulletinBoard", bulletinBoard, new Vector2(-418f, 180f), new Vector2(785f, 398f));

        if (printer != null)
            CreateAsset(canvas, "Printer", printer, new Vector2(-562f, -232f), new Vector2(296f, 367f));

        if (shredder != null)
            CreateAsset(canvas, "Shredder", shredder, new Vector2(576f, -215f), new Vector2(250f, 280f));

        if (flower != null)
            CreateAsset(canvas, "Flower", flower, new Vector2(-307f, -246f), new Vector2(94f, 210f));

        if (window != null)
            CreateAsset(canvas, "Window", window, new Vector2(354f, 142f), new Vector2(541f, 455f));

        if (ticket != null)
            CreateAsset(canvas, "Ticket", ticket, new Vector2(-406f, 262f), new Vector2(140f, 77f));

        // Laptop - created last so it renders on top
        if (laptop != null)
        {
            CreateAsset(canvas, "Laptop", laptop, new Vector2(0f, 0f), new Vector2(900f, 600f));
            Debug.Log("✓ Laptop created at center screen");
        }
        else
        {
            Debug.LogError("Laptop sprite is NULL - check Resources/UI_Assets/Laptop.png");
        }

        Debug.Log("✓ Scene setup complete!");
    }

    private Sprite LoadSpriteFromPath(string path)
    {
        // Try loading from Resources first
        string resourcePath = path.Replace("Assets/", "").Replace(".png", "");
        Sprite sprite = Resources.Load<Sprite>(resourcePath);

        if (sprite != null)
        {
            Debug.Log($"Loaded sprite from Resources: {resourcePath}");
            return sprite;
        }

        Debug.LogWarning($"Could not load sprite from {path}");
        return null;
    }

    private GameObject CreateAsset(Canvas canvas, string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;

        Debug.Log($"✓ Created {name}");
        return obj;
    }
}
