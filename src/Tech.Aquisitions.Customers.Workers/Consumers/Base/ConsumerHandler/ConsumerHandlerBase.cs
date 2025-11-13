using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Tech.Aquisitions.Customers.Workers.Consumers.Base.ConnectionManager.Interfaces;
using Tech.Aquisitions.Customers.Workers.Consumers.Base.Events;

namespace Tech.Aquisitions.Customers.Workers.Consumers.Base.ConsumerHandler;

public abstract class ConsumerHandlerBase<TEvent> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConsumerHandlerBase<TEvent>> _logger;
    private readonly IRabbitMqConnectionManager _connectionManager;

    protected ConsumerHandlerBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetRequiredService<ILogger<ConsumerHandlerBase<TEvent>>>();
        _connectionManager = _serviceProvider.GetRequiredService<IRabbitMqConnectionManager>();
    }

    protected abstract string QueueName { get; }

    protected virtual bool AutoAck => false;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await _connectionManager.GetResilientAlwaysOpennedConnectionAsync(stoppingToken);

        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (@object, @event) =>
        {
            var scope = _serviceProvider.CreateScope();

            var raw = @event.Body.ToArray();

            var input = JsonSerializer.Deserialize<IEvent<TEvent>>(raw, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            })!;

            await OnEventReceived(input, scope.ServiceProvider, stoppingToken);
        };

        var consumerTag = $"{QueueName}-{Guid.NewGuid().ToString()}-consumers";

        _ = await channel.BasicConsumeAsync(QueueName, AutoAck, consumerTag, consumer, stoppingToken);

        _logger.LogInformation(
            message: "[{Type}] The consumer has been started. Input = {@Input}",
            nameof(ConsumerHandlerBase<TEvent>),
            new 
            {
                ConsumerTag = consumerTag
            });
    }

    protected abstract Task OnEventReceived(IEvent<TEvent> @event, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
