using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Photo Reveal minigame: player hovers mouse over mosaic tiles to reveal a hidden image.
/// When X% of tiles are revealed, minigame completes.
/// </summary>
public class PhotoRevealMinigameUI : BaseMinigameUI
{
    public static PhotoRevealMinigameUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform imageContainer;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject tilePrefab;

    [Header("Settings")]
    [SerializeField] private int tileCountX = 6;
    [SerializeField] private int tileCountY = 6;
    [SerializeField] private float revealThreshold = 0.8f; // 80% revealed to complete
    [SerializeField] private Sprite revealSprite;

    // Tile data
    private class Tile
    {
        public Image image;
        public Color mosiacColor;
        public bool isRevealed = false;
    }

    private Tile[] tiles;
    private int totalTiles;
    private int revealedCount = 0;

    // Colors for mosaic effect
    private Color[] mosaicColors = new Color[]
    {
        new Color(0.4f, 0.3f, 0.2f, 1f), // Brown
        new Color(0.3f, 0.4f, 0.3f, 1f), // Olive
        new Color(0.2f, 0.3f, 0.4f, 1f), // Blue-gray
        new Color(0.4f, 0.2f, 0.3f, 1f), // Mauve
        new Color(0.3f, 0.3f, 0.3f, 1f), // Dark gray
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (minigameWindow != null)
        {
            minigameWindow.SetActive(false);
        }
    }

    /// <summary>
    /// Override StartMinigame to initialize the tile grid
    /// </summary>
    public override void StartMinigame(System.Action completionCallback)
    {
        onMinigameCompleted = completionCallback;
        isActive = true;
        isCompleted = false;
        revealedCount = 0;

        // Create tile grid
        CreateTileGrid();

        // Apply memory assist upgrade if active (pre-reveals 30% of tiles)
        // TODO: Integrate with new UpgradeService
        // if (UpgradeManager.Instance != null && UpgradeManager.Instance.IsMemoryAssistActive())
        // {
        //     PrerevealTiles();
        // }

        // Show window
        ShowWindow();

        UpdateProgress();
    }

    /// <summary>
    /// Create the NxN grid of tiles
    /// </summary>
    private void CreateTileGrid()
    {
        // Clear existing tiles
        if (imageContainer != null)
        {
            foreach (Transform child in imageContainer)
            {
                Destroy(child.gameObject);
            }
        }

        totalTiles = tileCountX * tileCountY;
        tiles = new Tile[totalTiles];
        revealedCount = 0;

        if (tilePrefab == null)
        {
            Debug.LogError("PhotoRevealMinigameUI: tilePrefab not assigned!");
            return;
        }

        for (int i = 0; i < totalTiles; i++)
        {
            GameObject tileObj = Instantiate(tilePrefab, imageContainer);
            Image tileImage = tileObj.GetComponent<Image>();

            if (tileImage == null)
            {
                tileImage = tileObj.AddComponent<Image>();
            }

            // Set up mosaic color
            Color mosiacColor = mosaicColors[i % mosaicColors.Length];
            tileImage.color = mosiacColor;

            // Add event trigger for hover
            EventTrigger trigger = tileObj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = tileObj.AddComponent<EventTrigger>();
            }

            // Pointer enter event
            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            int tileIndex = i;
            pointerEnterEntry.callback.AddListener((_) => OnTileHover(tileIndex));
            trigger.triggers.Add(pointerEnterEntry);

            // Store tile data
            Tile tile = new Tile();
            tile.image = tileImage;
            tile.mosiacColor = mosiacColor;
            tile.isRevealed = false;
            tiles[i] = tile;
        }
    }

    /// <summary>
    /// Pre-reveal some tiles (used by Memory Assist upgrade)
    /// </summary>
    private void PrerevealTiles()
    {
        int tileToReveal = Mathf.RoundToInt(totalTiles * 0.3f); // 30% pre-revealed

        List<int> indices = new List<int>();
        for (int i = 0; i < totalTiles; i++)
        {
            indices.Add(i);
        }

        // Fisher-Yates shuffle to pick random tiles
        for (int i = 0; i < tileToReveal && indices.Count > 0; i++)
        {
            int randomIdx = Random.Range(0, indices.Count);
            int tileIdx = indices[randomIdx];
            indices.RemoveAt(randomIdx);

            RevealTile(tileIdx);
        }
    }

    /// <summary>
    /// Called when player hovers over a tile
    /// </summary>
    private void OnTileHover(int tileIndex)
    {
        if (!isActive || isCompleted || tileIndex < 0 || tileIndex >= tiles.Length) return;

        RevealTile(tileIndex);
    }

    /// <summary>
    /// Reveal a single tile
    /// </summary>
    private void RevealTile(int tileIndex)
    {
        if (tiles[tileIndex].isRevealed) return;

        tiles[tileIndex].isRevealed = true;
        revealedCount++;

        // Update tile appearance to revealed state
        if (revealSprite != null)
        {
            tiles[tileIndex].image.sprite = revealSprite;
            tiles[tileIndex].image.color = Color.white; // Restore full color
        }
        else
        {
            // Fallback: just show white to indicate revealed
            tiles[tileIndex].image.color = Color.white;
        }

        // Check completion
        float revealPercentage = (float)revealedCount / totalTiles;
        if (revealPercentage >= revealThreshold && !isCompleted)
        {
            CompleteMinigame();
        }

        UpdateProgress();
    }

    /// <summary>
    /// Update progress display
    /// </summary>
    private void UpdateProgress()
    {
        if (progressText != null)
        {
            float percentage = totalTiles > 0 ? ((float)revealedCount / totalTiles) * 100f : 0f;
            progressText.text = $"{percentage:F0}% revealed";
        }
    }

    /// <summary>
    /// Override CloseMinigame to clean up tiles
    /// </summary>
    public override void CloseMinigame()
    {
        base.CloseMinigame();

        // Destroy all tiles
        if (imageContainer != null)
        {
            foreach (Transform child in imageContainer)
            {
                Destroy(child.gameObject);
            }
        }

        tiles = null;
    }
}
