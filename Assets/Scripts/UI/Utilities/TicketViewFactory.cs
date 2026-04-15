using UnityEngine;

public class TicketViewFactory
{
    private GameObject _ticketPrefab;
    private Transform _container;

    public TicketViewFactory(GameObject prefab, Transform container)
    {
        _ticketPrefab = prefab;
        _container = container;
    }

    public TicketView CreateTicketView(TicketModel model, System.Action onStartTask)
    {
        if (_ticketPrefab == null)
        {
            Debug.LogError("TicketViewFactory: Prefab is null");
            return null;
        }

        GameObject instance = Object.Instantiate(_ticketPrefab, _container);
        instance.name = $"Ticket_{model.Id}";

        TicketView view = instance.GetComponent<TicketView>();
        if (view == null)
            view = instance.AddComponent<TicketView>();

        view.SetupWithModel(model, onStartTask);
        return view;
    }
}
