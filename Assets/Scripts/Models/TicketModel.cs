using System;

[System.Serializable]
public class TicketModel : IEquatable<TicketModel>
{
    public string Id { get; private set; }
    public TaskData TaskData { get; private set; }
    public bool IsStamped { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public TicketModel(TaskData taskData)
    {
        Id = Guid.NewGuid().ToString();
        TaskData = taskData;
        IsStamped = false;
        CreatedAt = DateTime.UtcNow;
    }

    public TicketModel WithStamped(bool stamped)
    {
        var copy = new TicketModel(TaskData)
        {
            Id = this.Id,
            IsStamped = stamped,
            CreatedAt = this.CreatedAt
        };
        return copy;
    }

    public override bool Equals(object obj) => Equals(obj as TicketModel);
    public bool Equals(TicketModel other) => other != null && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
