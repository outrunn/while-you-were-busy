using System.Collections.Generic;
using UnityEngine;

public class TicketService
{
    private HashSet<TicketModel> _activeTickets = new HashSet<TicketModel>();
    private int _maxActiveTickets;

    public TicketService(int maxActive = 10)
    {
        _maxActiveTickets = maxActive;
    }

    public TicketModel CreateTicket(TaskData taskData)
    {
        if (_activeTickets.Count >= _maxActiveTickets)
        {
            Debug.LogWarning($"TicketService: Max active tickets ({_maxActiveTickets}) reached");
            return null;
        }

        TicketModel ticket = new TicketModel(taskData);
        _activeTickets.Add(ticket);
        return ticket;
    }

    public void StampTicket(TicketModel ticket)
    {
        if (_activeTickets.Contains(ticket))
        {
            TicketModel stamped = ticket.WithStamped(true);
            _activeTickets.Remove(ticket);
            _activeTickets.Add(stamped);

            GameEvents.Instance?.OnTicketStamped.Invoke(stamped);
        }
    }

    public void ShredTicket(TicketModel ticket)
    {
        if (_activeTickets.Contains(ticket))
        {
            _activeTickets.Remove(ticket);
            GameEvents.Instance?.OnTicketShredded.Invoke(ticket);
        }
    }

    public int GetActiveCount() => _activeTickets.Count;
    public bool HasRoom() => _activeTickets.Count < _maxActiveTickets;
}
