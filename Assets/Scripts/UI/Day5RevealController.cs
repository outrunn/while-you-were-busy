using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Day5RevealController : MonoBehaviour
{
    [SerializeField] private GameObject ticketPrefabPath = null;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject endingPanelPrefab;
    [SerializeField] private float ticketSpawnRate = 0.3f; // Spawn every 0.3 seconds
    [SerializeField] private int ticketsToSpawn = 30; // Spawn 30 tickets
    [SerializeField] private float revealDelay = 3f; // Wait 3 seconds then reveal

    private bool _isRunning = false;
    private List<GameObject> _spawnedTickets = new List<GameObject>();

    public void StartDay5Reveal()
    {
        if (_isRunning) return;
        _isRunning = true;

        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        if (spawnArea == null)
        {
            // Default to canvas if no spawn area specified
            spawnArea = canvas.transform;
        }

        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        // Pause game time to stop normal gameplay
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.5f);

        // Resume for ticket spawning effect
        Time.timeScale = 1f;

        // Spawn tickets rapidly
        yield return StartCoroutine(SpawnTicketsSequence());

        // Wait for reveal
        yield return new WaitForSeconds(revealDelay);

        // Pause again
        Time.timeScale = 0f;

        // Show ending panel
        ShowEndingPanel();
    }

    private IEnumerator SpawnTicketsSequence()
    {
        for (int i = 0; i < ticketsToSpawn; i++)
        {
            SpawnRandomTicket();
            yield return new WaitForSeconds(ticketSpawnRate);
        }
    }

    private void SpawnRandomTicket()
    {
        // Load ticket prefab from resources
        GameObject ticketPrefab = Resources.Load<GameObject>("Prefabs/Tickets/Ticket");
        if (ticketPrefab == null)
        {
            Debug.LogWarning("[Day5RevealController] Ticket prefab not found at Resources/Prefabs/Tickets/Ticket");
            return;
        }

        // Spawn at random position in canvas
        Vector3 randomPos = new Vector3(
            Random.Range(-300f, 300f),
            Random.Range(-200f, 200f),
            0
        );

        GameObject ticket = Instantiate(ticketPrefab, spawnArea);
        RectTransform rect = ticket.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = randomPos;
        }

        _spawnedTickets.Add(ticket);
    }

    private void ShowEndingPanel()
    {
        if (endingPanelPrefab == null)
        {
            endingPanelPrefab = Resources.Load<GameObject>("Prefabs/UI/EndingPanel");
        }

        if (endingPanelPrefab == null)
        {
            Debug.LogError("[Day5RevealController] Ending panel prefab not found!");
            return;
        }

        GameObject panelInstance = Instantiate(endingPanelPrefab, canvas.transform);
        EndingPanelUI panelUI = panelInstance.GetComponent<EndingPanelUI>();

        if (panelUI != null)
        {
            // Load the ending background from resources
            Sprite endingBackground = Resources.Load<Sprite>("UI_Assets/EndingPanelBackground");
            string endingMessage = "While you were busy...\nYou were never human.";
            panelUI.PlayEnding(endingBackground, endingMessage);
        }
    }

    public void CleanupTickets()
    {
        foreach (var ticket in _spawnedTickets)
        {
            if (ticket != null)
                Destroy(ticket);
        }
        _spawnedTickets.Clear();
    }
}
