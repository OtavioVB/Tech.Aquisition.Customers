using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher.Interfaces;

public interface IRabbitMqPublisher
{
    public Task PublishAsync<T>(IEvent<T> @event, string exchangeName, string routingKey, CancellationToken cancellationToken = default);
}
