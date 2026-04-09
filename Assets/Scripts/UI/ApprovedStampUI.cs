using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the approved stamp animation and visual feedback when tickets are completed.
/// </summary>
public class ApprovedStampUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float stampDuration = 0.5f;
    [SerializeField] private float stampScale = 1.2f;
    [SerializeField] private CanvasGroup canvasGroup;

    private Image stampImage;
    private RectTransform stampTransform;

    private void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        stampImage = GetComponent<Image>();
        stampTransform = GetComponent<RectTransform>();

        // Start invisible
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Play stamp animation when ticket is approved
    /// </summary>
    public void PlayStampAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(StampAnimationRoutine());
    }

    private IEnumerator StampAnimationRoutine()
    {
        // Fade in while scaling
        canvasGroup.alpha = 1;
        stampTransform.localScale = Vector3.one * stampScale;

        float elapsed = 0f;
        while (elapsed < stampDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / stampDuration;

            // Scale down
            stampTransform.localScale = Vector3.one * Mathf.Lerp(stampScale, 1f, progress);

            yield return null;
        }

        stampTransform.localScale = Vector3.one;

        // Keep visible for a moment then fade out
        yield return new WaitForSeconds(0.5f);

        elapsed = 0f;
        float fadeDuration = 0.3f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, progress);

            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}
