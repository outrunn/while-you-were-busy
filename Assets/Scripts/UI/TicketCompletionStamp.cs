using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the approved stamp animation that appears over a completed ticket.
/// Creates and animates the stamp on demand when a ticket is completed.
/// </summary>
public class TicketCompletionStamp : MonoBehaviour
{
    [Header("Stamp Settings")]
    [SerializeField] private Sprite stampSprite;
    [SerializeField] private float stampDuration = 1.5f;
    [SerializeField] private float scaleStart = 1.5f;
    [SerializeField] private float scaleFinal = 0.8f;
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    [SerializeField] private Vector2 stampSize = new Vector2(150, 150);

    /// <summary>
    /// Show approval stamp over a ticket position
    /// </summary>
    public void ShowStampAtTicket(RectTransform ticketRect)
    {
        if (stampSprite == null)
        {
            Debug.LogWarning("TicketCompletionStamp: Stamp sprite not assigned!");
            return;
        }

        StartCoroutine(StampAnimationRoutine(ticketRect));
    }

    private IEnumerator StampAnimationRoutine(RectTransform ticketRect)
    {
        // Create stamp GameObject
        GameObject stampObj = new GameObject("ApprovedStamp_Temp");
        stampObj.transform.SetParent(ticketRect.parent, false); // Same parent as ticket

        RectTransform stampRect = stampObj.AddComponent<RectTransform>();
        stampRect.anchoredPosition = ticketRect.anchoredPosition;
        stampRect.sizeDelta = stampSize;
        stampRect.localScale = Vector3.one * scaleStart;

        Image stampImage = stampObj.AddComponent<Image>();
        stampImage.sprite = stampSprite;
        stampImage.preserveAspect = true;

        CanvasGroup canvasGroup = stampObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        float elapsed = 0f;
        float rotation = 0f;

        // Animate
        while (elapsed < stampDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / stampDuration;

            // Scale down
            float scale = Mathf.Lerp(scaleStart, scaleFinal, progress);
            stampRect.localScale = Vector3.one * scale;

            // Rotate
            rotation += rotationSpeed * Time.deltaTime;
            stampRect.localEulerAngles = new Vector3(0, 0, rotation);

            // Fade out in final 0.3s
            if (progress > 0.7f)
            {
                float fadeProgress = (progress - 0.7f) / 0.3f;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        // Cleanup
        Destroy(stampObj);
    }
}
