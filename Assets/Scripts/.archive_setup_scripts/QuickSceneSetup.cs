using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick setup script that positions all assets to match the target scene layout.
/// Run this once to arrange everything, then adjust as needed.
/// </summary>
public class QuickSceneSetup : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Image bulletinBoardImage;
    [SerializeField] private Image printerImage;
    [SerializeField] private Image shredderImage;
    [SerializeField] private Image flowerImage;
    [SerializeField] private Image windowImage;
    [SerializeField] private Image ticketImage;
    [SerializeField] private Image stampImage;

    [ContextMenu("Setup Scene Layout")]
    public void SetupLayout()
    {
        if (mainCanvas == null)
            mainCanvas = FindFirstObjectByType<Canvas>();

        // Bulletin Board
        PositionElement(bulletinBoardImage, new Vector2(-418f, 180f), new Vector2(785f, 398f));

        // Printer
        PositionElement(printerImage, new Vector2(-562f, -232f), new Vector2(296f, 367f));

        // Shredder
        PositionElement(shredderImage, new Vector2(576f, -215f), new Vector2(250f, 280f));

        // Flower
        PositionElement(flowerImage, new Vector2(-307f, -246f), new Vector2(94f, 210f));

        // Window
        PositionElement(windowImage, new Vector2(354f, 142f), new Vector2(541f, 455f));

        // Ticket
        PositionElement(ticketImage, new Vector2(-406f, 262f), new Vector2(140f, 77f));

        // Stamp - HIDDEN (show on completion)
        if (stampImage != null)
        {
            stampImage.gameObject.SetActive(false);
        }

        Debug.Log("Scene layout configured!");
    }

    private void PositionElement(Image image, Vector2 position, Vector2 size)
    {
        if (image == null) return;

        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        image.preserveAspect = true;
    }
}
