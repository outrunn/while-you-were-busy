using UnityEngine;
using UnityEngine.UI;

public class UIElementSetup : MonoBehaviour
{
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null) return;

        // Ensure ScreenSpaceOverlay mode
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Load sprites from Resources/UI_Assets
        Sprite bgSprite = Resources.Load<Sprite>("UI_Assets/Rectangle 45");
        Sprite deskSprite = Resources.Load<Sprite>("UI_Assets/image 5");
        Sprite bulletinSprite = Resources.Load<Sprite>("UI_Assets/Bulletin Board");
        Sprite windowSprite = Resources.Load<Sprite>("UI_Assets/Window");
        Sprite flowerSprite = Resources.Load<Sprite>("UI_Assets/Flower");
        Sprite shredderSprite = Resources.Load<Sprite>("UI_Assets/Shredder");

        // Create Background (fills entire screen)
        CreateElement(canvas, "Background", bgSprite, true, new Vector2(1920, 1080));

        // Create other elements at origin with original sizes
        CreateElement(canvas, "Desk", deskSprite, false, new Vector2(1024, 512));
        CreateElement(canvas, "BulletinBoard", bulletinSprite, false, new Vector2(512, 512));
        CreateElement(canvas, "Window", windowSprite, false, new Vector2(300, 180));
        CreateElement(canvas, "Flower", flowerSprite, false, new Vector2(64, 128));
        CreateElement(canvas, "Shredder", shredderSprite, false, new Vector2(200, 200));

        Debug.Log("UI elements created - ready for positioning");
    }

    private void CreateElement(Canvas canvas, string name, Sprite sprite, bool fillScreen, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(canvas.transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();

        if (fillScreen)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.sizeDelta = size;
            rt.anchoredPosition = Vector2.zero;
        }

        Image img = obj.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = false;

        Debug.Log($"Created {name} with size {size}");
    }
}
