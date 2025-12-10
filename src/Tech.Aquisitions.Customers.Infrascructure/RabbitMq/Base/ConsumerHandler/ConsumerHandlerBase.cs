using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager.Interfaces;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConsumerHandler;

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

        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (@object, @event) =>
        {
            var scope = _serviceProvider.CreateScope();

            var raw = @event.Body.ToArray();

            try
            {
                var input = JsonSerializer.Deserialize<IEvent<TEvent>>(raw, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                })!;

                await OnEventReceived(input, scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                var headers = @event.BasicProperties.Headers;

                headers!.Add("X-Stack-Trace", ex.StackTrace);
                headers.Add("X-Error-Message", ex.Message);

                await channel.BasicPublishAsync(
                    exchange: $"{@event.Exchange}.dead",
                    routingKey: @event.RoutingKey,
                    body: @event.Body.ToArray(),
                    mandatory: false,
                    basicProperties: new BasicProperties()
                    {
                        Headers = headers
                    },
                    cancellationToken: stoppingToken);

                _logger.LogError(
                    exception: ex,
                    message: "[{Type}] Got an exception handling event receveiment. Input = {@Input}",
                    nameof(ConsumerHandlerBase<TEvent>),
                    new
                    {
                        @event.Exchange,
                        @event.RoutingKey
                    });
            }
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
