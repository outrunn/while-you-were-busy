using UnityEngine;

public class PrinterSetup : MonoBehaviour
{
    private void Awake()
    {
        // Find PrinterController on Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        PrinterController printer = canvas.GetComponentInChildren<PrinterController>();
        if (printer == null) return;

        // Find TicketSpawnPoint
        Transform ticketSpawnPoint = canvas.transform.Find("Printer/TicketSpawnPoint");
        if (ticketSpawnPoint == null)
            ticketSpawnPoint = canvas.transform.Find("TicketSpawnPoint");

        // Find or create TicketSpawnPoint if it doesn't exist
        if (ticketSpawnPoint == null)
        {
            GameObject spawnPointObj = new GameObject("TicketSpawnPoint");
            spawnPointObj.transform.SetParent(printer.transform);
            ticketSpawnPoint = spawnPointObj.transform;
            RectTransform rect = spawnPointObj.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 0);
        }

        // Use reflection to set the private field
        var field = typeof(PrinterController).GetField("ticketSpawnPoint", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(printer, ticketSpawnPoint);

        Debug.Log("[PrinterSetup] Set ticketSpawnPoint on PrinterController");

        // Destroy this setup script after use
        Destroy(this);
    }
}
