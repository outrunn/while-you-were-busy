using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TicketView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button startTaskButton;
    [SerializeField] private Image stampImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private TicketModel _model;
    private RectTransform _rect;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isExpanded = false;
    private Coroutine _expandCoroutine;

    public TicketModel Model => _model;

    public void SetupWithModel(TicketModel model, System.Action onStartTask)
    {
        _model = model;
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>().rootCanvas;

        // Setup UI
        if (titleText != null)
            titleText.text = model.TaskData.Title;
        if (descriptionText != null)
            descriptionText.text = model.TaskData.Description;

        // Setup button
        if (startTaskButton != null)
        {
            startTaskButton.gameObject.SetActive(model.TaskData.RequiresMinigame);
            startTaskButton.onClick.AddListener(() => onStartTask?.Invoke());
        }

        if (stampImage != null)
            stampImage.gameObject.SetActive(false);

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void UpdateModel(TicketModel newModel)
    {
        _model = newModel;

        if (newModel.IsStamped)
        {
            ShowStamp();
            if (startTaskButton != null)
                startTaskButton.gameObject.SetActive(false);
        }
    }

    public void ShowStamp()
    {
        if (stampImage != null)
            stampImage.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_model.IsStamped)
            ToggleExpand();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalPosition = _rect.anchoredPosition;
        _originalParent = transform.parent;

        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.worldCamera,
            out Vector2 localPoint);
        _rect.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check for shredder drop
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = _canvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                ShredderDropZone shredder = result.gameObject.GetComponent<ShredderDropZone>();
                if (shredder != null && _model.IsStamped)
                {
                    shredder.OnTicketDropped(_model);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // No valid drop - return to original position
        transform.SetParent(_originalParent);
        _rect.anchoredPosition = _originalPosition;
    }

    private void ToggleExpand()
    {
        if (_expandCoroutine != null)
            StopCoroutine(_expandCoroutine);

        _expandCoroutine = StartCoroutine(ExpandCoroutine(!_isExpanded));
    }

    private IEnumerator ExpandCoroutine(bool shouldExpand)
    {
        if (shouldExpand && _canvas != null)
        {
            transform.SetParent(_canvas.transform);
            transform.SetAsLastSibling();
        }

        Vector3 startScale = _rect.localScale;
        Vector3 endScale = shouldExpand ? Vector3.one : new Vector3(Constants.TICKET_MINI_SCALE, Constants.TICKET_MINI_SCALE, 1f);

        float elapsed = 0f;
        while (elapsed < Constants.TICKET_EXPAND_DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / Constants.TICKET_EXPAND_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            _rect.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        _rect.localScale = endScale;
        _isExpanded = shouldExpand;
    }
}
