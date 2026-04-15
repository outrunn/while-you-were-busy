using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EndingPanelUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private float fadeInDuration = 2f;

    private bool _isPlaying = false;

    public void PlayEnding(Sprite backgroundSprite, string endingMessage = "")
    {
        if (_isPlaying) return;
        _isPlaying = true;

        if (backgroundImage != null && backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }

        if (endingText != null && !string.IsNullOrEmpty(endingMessage))
        {
            endingText.text = endingMessage;
        }

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
