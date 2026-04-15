using UnityEngine;

public class EndingController : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;

    public void PlayEnding()
    {
        if (endingPanel != null)
            endingPanel.SetActive(true);
    }

    public void Close()
    {
        Time.timeScale = 1f;
        if (endingPanel != null)
            endingPanel.SetActive(false);
        Destroy(gameObject);
    }
}
