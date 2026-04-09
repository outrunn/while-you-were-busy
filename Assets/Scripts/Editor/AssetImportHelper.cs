using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Helper tool to import and configure UI assets from the UI_Assets folder
/// </summary>
public class AssetImportHelper
{
    private static string ASSETS_FOLDER = "Assets/UI_Assets";
    private static string SPRITES_FOLDER = "Assets/Sprites";

    [MenuItem("Tools/Setup Scene Assets from UI_Assets")]
    public static void SetupSceneAssets()
    {
        // Find all PNG files in the UI_Assets folder
        string[] assetPaths = Directory.GetFiles(Path.Combine(Application.dataPath, "UI_Assets"), "*.png");

        if (assetPaths.Length == 0)
        {
            EditorUtility.DisplayDialog("Asset Import", "No PNG files found in Assets/UI_Assets", "OK");
            return;
        }

        // Create Sprites folder if it doesn't exist
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Sprites")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sprites"));
            AssetDatabase.Refresh();
        }

        // Move files to Sprites folder and configure them
        foreach (string assetPath in assetPaths)
        {
            string fileName = Path.GetFileName(assetPath);
            string destPath = Path.Combine(Application.dataPath, "Sprites", fileName);

            if (!File.Exists(destPath))
            {
                File.Copy(assetPath, destPath);
            }

            ConfigureSprite($"Assets/Sprites/{fileName}");
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Asset Import", $"Successfully imported {assetPaths.Length} sprites", "OK");
    }

    private static void ConfigureSprite(string spritePath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;

        // Set compression for UI sprites
        importer.textureCompression = TextureImporterCompression.Compressed;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    [MenuItem("Tools/Create Scene from Imported Assets")]
    public static void CreateSceneWithAssets()
    {
        // Create a new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Create Canvas
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObject.AddComponent<GraphicRaycaster>();

        // Add SceneAssetSetup component
        SceneAssetSetup setupComponent = canvasObject.AddComponent<SceneAssetSetup>();

        // Load and assign sprites
        AssignSpritesToComponent(setupComponent);

        // Save scene
        string scenePath = "Assets/Scenes/GameScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);

        EditorUtility.DisplayDialog("Scene Created", $"Scene created at {scenePath}", "OK");
    }

    private static void AssignSpritesToComponent(SceneAssetSetup setupComponent)
    {
        // This would need reflection or a more complex setup
        // For now, user can manually assign sprites in the inspector
        Debug.Log("Please assign sprites to the SceneAssetSetup component in the inspector");
    }
}
