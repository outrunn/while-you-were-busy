using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Scene editor - allows dragging and resizing UI assets in the editor or play mode.
/// Select an asset and use mouse to move/resize.
/// </summary>
public class SceneEditor : MonoBehaviour
{
    [Header("Editor Settings")]
    [SerializeField] private bool editorModeEnabled = true;
    [SerializeField] private float resizeHandleSize = 20f; // TODO: implement visual resize handles
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color defaultColor = Color.white;

    private RectTransform selectedRectTransform;
    private Image selectedImage;
    private Vector2 dragOffset;
    private bool isDragging = false;
    private bool isResizing = false;
    private Vector2 resizeStartSize;
    private Vector2 resizeStartMousePos;

    private void Update()
    {
        if (!editorModeEnabled) return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Left click to select/drag
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector3.forward);

            RectTransform rectUnderMouse = GetRectTransformAtMousePosition();

            if (rectUnderMouse != null && rectUnderMouse.GetComponent<Image>() != null)
            {
                SelectAsset(rectUnderMouse);
                isDragging = true;
                dragOffset = (Vector2)rectUnderMouse.position - (Vector2)Input.mousePosition;
            }
            else
            {
                DeselectAsset();
            }
        }

        // Dragging
        if (Input.GetMouseButton(0) && isDragging && selectedRectTransform != null)
        {
            Vector2 mousePos = Input.mousePosition;
            selectedRectTransform.position = mousePos + dragOffset;
        }

        // Release mouse
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Right click to resize
        if (Input.GetMouseButtonDown(1))
        {
            RectTransform rectUnderMouse = GetRectTransformAtMousePosition();
            if (rectUnderMouse != null && selectedRectTransform == rectUnderMouse)
            {
                isResizing = true;
                resizeStartSize = selectedRectTransform.sizeDelta;
                resizeStartMousePos = Input.mousePosition;
            }
        }

        // Resizing
        if (Input.GetMouseButton(1) && isResizing && selectedRectTransform != null)
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - resizeStartMousePos;
            selectedRectTransform.sizeDelta = resizeStartSize + mouseDelta;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isResizing = false;
        }

        // Delete key to remove selected asset
        if (Input.GetKeyDown(KeyCode.Delete) && selectedRectTransform != null)
        {
            Debug.Log($"Deleting {selectedRectTransform.name}");
            Destroy(selectedRectTransform.gameObject);
            selectedRectTransform = null;
            selectedImage = null;
        }

        // Arrow keys to nudge position
        if (selectedRectTransform != null)
        {
            Vector2 nudge = Vector2.zero;
            if (Input.GetKey(KeyCode.UpArrow)) nudge.y += 10f;
            if (Input.GetKey(KeyCode.DownArrow)) nudge.y -= 10f;
            if (Input.GetKey(KeyCode.LeftArrow)) nudge.x -= 10f;
            if (Input.GetKey(KeyCode.RightArrow)) nudge.x += 10f;

            if (nudge != Vector2.zero)
            {
                selectedRectTransform.anchoredPosition += nudge * Time.deltaTime * 100f;
            }
        }

        // Log selected asset info
        if (Input.GetKeyDown(KeyCode.I) && selectedRectTransform != null)
        {
            LogAssetInfo();
        }
    }

    private RectTransform GetRectTransformAtMousePosition()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            RectTransform rect = result.gameObject.GetComponent<RectTransform>();
            if (rect != null && rect.GetComponent<Image>() != null)
            {
                return rect;
            }
        }

        return null;
    }

    private void SelectAsset(RectTransform rect)
    {
        if (selectedRectTransform != null && selectedImage != null)
        {
            selectedImage.color = defaultColor;
        }

        selectedRectTransform = rect;
        selectedImage = rect.GetComponent<Image>();

        if (selectedImage != null)
        {
            selectedImage.color = selectedColor;
        }

        Debug.Log($"Selected: {rect.name} | Position: {rect.anchoredPosition} | Size: {rect.sizeDelta}");
    }

    private void DeselectAsset()
    {
        if (selectedImage != null)
        {
            selectedImage.color = defaultColor;
        }
        selectedRectTransform = null;
        selectedImage = null;
    }

    private void LogAssetInfo()
    {
        if (selectedRectTransform != null)
        {
            Debug.Log($"\n=== {selectedRectTransform.name} ===\n" +
                     $"Position: {selectedRectTransform.anchoredPosition}\n" +
                     $"Size: {selectedRectTransform.sizeDelta}\n" +
                     $"Scale: {selectedRectTransform.localScale}\n" +
                     $"Rotation: {selectedRectTransform.localEulerAngles}\n");
        }
    }

    /// <summary>
    /// Print all asset positions and sizes for easy copying
    /// </summary>
    [ContextMenu("Print All Asset Positions")]
    public void PrintAllAssetPositions()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();

        Debug.Log("\n=== SCENE ASSET POSITIONS ===");
        foreach (Transform child in canvas.transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null && child.GetComponent<Image>() != null)
            {
                Debug.Log($"CreateAsset(\"{child.name}\", {child.name.ToLower()}Sprite, new Vector2({rect.anchoredPosition.x}f, {rect.anchoredPosition.y}f), new Vector2({rect.sizeDelta.x}f, {rect.sizeDelta.y}f));");
            }
        }
    }
}
