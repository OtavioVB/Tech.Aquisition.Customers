using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager.Interfaces;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    protected readonly ILogger<RabbitMqPublisher> _logger;
    protected readonly IRabbitMqConnectionManager _connectionManager;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, IRabbitMqConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
    }

    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(_connectionMaxAccessLockCount);
    private const int _connectionMaxAccessLockCount = 1;

    protected IChannel? _channel;

    private async Task<IChannel> TryCreateChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_channel != null)
            return _channel;

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            var connection = await _connectionManager.GetResilientAlwaysOpennedConnectionAsync(cancellationToken);

            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return _channel;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public virtual async Task PublishAsync<T>(IEvent<T> @event, string exchangeName, string routingKey, CancellationToken cancellationToken = default)
    {
        var channel = await TryCreateChannelAsync(cancellationToken);

        var headers = new Dictionary<string, object?>()
        {
            { EventTag.Id, @event.EventId.ToString() },
            { EventTag.Timestamp, @event.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") },
            { EventTag.Origin, Namespace.Name },
            { EventTag.RoutingKey, routingKey },
            { EventTag.Exchange, exchangeName }
        };

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: new BasicProperties()
            {
                Headers = headers
            },
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)));
    }
}
