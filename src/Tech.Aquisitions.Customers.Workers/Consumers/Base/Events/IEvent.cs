namespace Tech.Aquisitions.Customers.Workers.Consumers.Base.Events;

public interface IEvent<T>
{
    public Guid EventId { get; set; }
    public T Event { get; set; }
    public DateTime Timestamp { get; set; }
}
