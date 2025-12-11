namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

public interface IEvent<T>
{
    public Guid EventId { get; set; }
    public string? Origin { get; set; }
    public string? RoutingKey { get; set; }
    public T Event { get; set; }
    public DateTime Timestamp { get; set; }
}
