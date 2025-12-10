using RabbitMQ.Client;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager.Interfaces;

public interface IRabbitMqConnectionManager
{
    public Task<IConnection> GetResilientAlwaysOpennedConnectionAsync(CancellationToken cancellationToken = default);
    public Task<IConnection> TryConnectAsync(CancellationToken cancellationToken = default);
}
