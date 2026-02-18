using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls background color based on world health.
/// Provides visual feedback through gradual color changes.
/// </summary>
public class BackgroundController : MonoBehaviour
{
    [Header("Background Reference")]
    [SerializeField] private Image backgroundImage;

    [Header("Color Settings")]
    [SerializeField] private Color healthyColor = new Color(0.8f, 0.9f, 1f); // Light blue
    [SerializeField] private Color degradedColor = new Color(0.6f, 0.6f, 0.65f); // Desaturated
    [SerializeField] private Color criticalColor = new Color(0.3f, 0.3f, 0.35f); // Dark
    [SerializeField] private Color collapsedColor = new Color(0.1f, 0.1f, 0.15f); // Very dark

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 0.5f; // How fast colors lerp

    private Color targetColor;
    private Color currentColor;

    private void Start()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        if (backgroundImage != null)
        {
            currentColor = healthyColor;
            backgroundImage.color = currentColor;
        }
    }

    private void Update()
    {
        if (backgroundImage == null || GameManager.Instance == null) return;

        // Determine target color based on world health
        float worldHealth = GameManager.Instance.GetWorldHealth();
        targetColor = GetColorForHealth(worldHealth);

        // Smoothly transition to target color
        currentColor = Color.Lerp(currentColor, targetColor, transitionSpeed * Time.deltaTime);
        backgroundImage.color = currentColor;
    }

    /// <summary>
    /// Returns the appropriate color based on world health value
    /// </summary>
    private Color GetColorForHealth(float health)
    {
        if (health >= 70f)
        {
            // Healthy - slight gradient from perfect to healthy
            return Color.Lerp(degradedColor, healthyColor, (health - 70f) / 30f);
        }
        else if (health >= 40f)
        {
            // Degraded - desaturate
            return Color.Lerp(criticalColor, degradedColor, (health - 40f) / 30f);
        }
        else if (health >= 15f)
        {
            // Critical - darken significantly
            return Color.Lerp(collapsedColor, criticalColor, (health - 15f) / 25f);
        }
        else
        {
            // Collapsed - very dark
            return collapsedColor;
        }
    }
}
