using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ShredderDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.6f;

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Image image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            image.raycastTarget = true;
        }
    }

    public void OnTicketDropped(TicketModel ticket)
    {
        Debug.Log($"Shredded ticket: {ticket.TaskData.Title}");

        // Notify services that ticket was shredded
        GameEvents.Instance?.OnTicketShredded.Invoke(ticket);

        // Play shred animation
        StartCoroutine(ShredAnimation());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = activeAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = inactiveAlpha;
    }

    private IEnumerator ShredAnimation()
    {
        // Could add particle effects, rotation, sound here
        Debug.Log("Shredding animation...");
        yield return new WaitForSeconds(0.5f);
    }
}
