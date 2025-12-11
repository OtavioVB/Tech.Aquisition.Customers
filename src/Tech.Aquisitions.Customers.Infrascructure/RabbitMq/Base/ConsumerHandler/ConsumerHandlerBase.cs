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
    protected virtual bool EnableDlq => false;

    protected virtual async Task ProduceDlqAsync(Exception ex, BasicDeliverEventArgs @event, CancellationToken cancellationToken)
    {
        var connectionDlq = await _connectionManager.GetResilientAlwaysOpennedConnectionAsync(cancellationToken);

        await using var channelDlq = await connectionDlq.CreateChannelAsync(cancellationToken: cancellationToken);

        var headers = @event.BasicProperties.Headers;

        var exchangeDead = $"{@event.Exchange}.dead";
        var routingKey = @event.RoutingKey;

        headers?.Add(EventTag.StackTrace, ex.StackTrace);
        headers?.Add(EventTag.ErrorMessage, ex.Message);
        headers?.Add(EventTag.DlqPublishedTimestamp, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
        headers?.Add(EventTag.DeadExchange, exchangeDead);
        headers?.Add(EventTag.DeadRoutingKey, routingKey);

        await channelDlq.BasicPublishAsync(
            exchange: exchangeDead,
            routingKey: routingKey,
            body: @event.Body.ToArray(),
            mandatory: false,
            basicProperties: new BasicProperties()
            {
                Headers = headers
            },
            cancellationToken: cancellationToken);

        var eventId = @event.BasicProperties.Headers?[EventTag.Id]?.ToString();

        _logger.LogError(
            exception: ex,
            message: "[{Type}] The event has been published to DLQ queue, because, has got an exception handling event receveiment. Input = {@Input}",
            nameof(ConsumerHandlerBase<TEvent>),
            new
            {
                EventId = eventId,
                @event.DeliveryTag,
                @event.Exchange,
                @event.RoutingKey,
                DeadExchange = exchangeDead,
                DeadRoutingKey = routingKey
            });
    }

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
                var input = JsonSerializer.Deserialize<EventBase<TEvent>>(raw, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                })!;

                await OnEventReceived(input, scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                if (EnableDlq)
                {
                    await ProduceDlqAsync(ex, @event, stoppingToken);
                }
                else
                {
                    var eventId = @event.BasicProperties.Headers?["X-Event-Id"]?.ToString();

                    _logger.LogError("[{Type}] Got an exception handling event. The message will be discarted because the consumer doesn't have resilience policy activated. Input = {@Input}",
                        nameof(ConsumerHandlerBase<TEvent>),
                        new
                        {
                            EventId = eventId,
                            @event.DeliveryTag,
                            @event.Exchange,
                            @event.RoutingKey
                        });
                }
            }
            finally
            {
                if (AutoAck == false)
                {
                    await channel.BasicAckAsync(@event.DeliveryTag, multiple: false, stoppingToken);
                }
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
