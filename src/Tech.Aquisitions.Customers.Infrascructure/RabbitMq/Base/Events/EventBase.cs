namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

public class EventBase<T> : IEvent<T>
{
    public EventBase()
    {
    }

    public EventBase(T @event)
    {
        EventId = Guid.NewGuid();
        Event = @event;
        Timestamp = DateTime.UtcNow;
    }

    public EventBase(Guid eventId, T @event, DateTime timestamp, string? routingKey = null, string? origin = null)
    {
        EventId = eventId;
        Event = @event;
        Timestamp = timestamp;
        RoutingKey = routingKey;
        Origin = origin;
    }

    public Guid EventId { get; set; }
    public string? Origin { get; set; }
    public string? RoutingKey { get; set; }

    public T Event { get; set; }
    public DateTime Timestamp { get; set; }
}
