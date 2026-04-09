using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Setup laptop as a centered UI element on the Canvas.
/// Loads from Resources and displays at screen center.
/// </summary>
public class LaptopSetup : MonoBehaviour
{
    [SerializeField] private Sprite laptopSprite;

    private void Start()
    {
        if (laptopSprite != null)
        {
            CreateLaptop();
        }
        else
        {
            Debug.LogWarning("LaptopSetup: No laptop sprite assigned in Inspector!");
        }
    }

    private void CreateLaptop()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("LaptopSetup: No Canvas found!");
            return;
        }

        // Create laptop GameObject
        GameObject laptopObj = new GameObject("Laptop");
        laptopObj.transform.SetParent(canvas.transform, false);

        // Add RectTransform
        RectTransform rect = laptopObj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, 0f);  // Center
        rect.sizeDelta = new Vector2(900f, 600f);

        // Add Image
        Image image = laptopObj.AddComponent<Image>();
        image.sprite = laptopSprite;
        image.preserveAspect = true;

        Debug.Log("✓ Laptop created at center screen");
    }
}
