using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConsumerHandler;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

namespace Tech.Aquisitions.Customers.Workers.Consumers;

public sealed class AquisitionCustomerRequestedConsumer : ConsumerHandlerBase<AquisitionCustomerRequestedEvent>
{
    private readonly ILogger<AquisitionCustomerRequestedConsumer> _logger;

    public AquisitionCustomerRequestedConsumer(
        ILogger<AquisitionCustomerRequestedConsumer> logger, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _logger = logger;
    }

    protected override string QueueName => "tech.aquitions.customers.requested";

    protected override Task OnEventReceived(IEvent<AquisitionCustomerRequestedEvent> @event, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[{Type}] Event received. Input = {@Input}",
            nameof(AquisitionCustomerRequestedConsumer),
            new 
            {
                @event.EventId,
                @event.Timestamp
            });

        return Task.CompletedTask;
    }
}
