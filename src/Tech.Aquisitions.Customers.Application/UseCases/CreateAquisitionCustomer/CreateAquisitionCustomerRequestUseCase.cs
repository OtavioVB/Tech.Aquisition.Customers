using Microsoft.Extensions.Logging;
using Tech.Aquisitions.Customers.Application.UseCases.Base;
using Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer.Inputs;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;
using Tech.Aquisitions.Customers.Infrascructure;
using Tech.Aquisitions.Customers.Infrascructure.FeatureManager.Interfaces;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher.Interfaces;
using Tech.Aquisitions.Customers.Workers.Consumers;

namespace Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer;

public sealed class CreateAquisitionCustomerRequestUseCase : FeaturedUseCase<CreateAquisitionCustomerRequestUseCaseInput>
{
    private readonly IRabbitMqPublisher _publisher;

    public CreateAquisitionCustomerRequestUseCase(
        IRabbitMqPublisher publisher,
        ILogger<FeaturedUseCase<CreateAquisitionCustomerRequestUseCaseInput>> logger, 
        IFeatureManagement featureManagement) 
        : base(logger, featureManagement)
    {
        _publisher = publisher;
    }

    protected override string FeatureName => nameof(CreateAquisitionCustomerRequestUseCase);

    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_CODE = "CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR";
    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_MESSAGE = "Não foi possível solicitar a criação do cadastro de cliente.";

    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_SUCCESS_NOTIFICATION_CODE = "CREATE_AQUISITION_CUSTOMER_REQUEST_SUCCESS";
    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_SUCCESS_NOTIFICATION_MESSAGE = "A solicitação de aquisição de cliente foi realizada com sucesso.";

    private const string CREATE_AQUISITION_CUSTOMER_EXCHANGE_NAME = "tech-aquisition-customers";
    private const string CREATE_AQUISITION_CUSTOMER_ROUTING_KEY = "tech.aquitions.customers.request";

    protected override async Task<MethodResult> HandleAsync(CreateAquisitionCustomerRequestUseCaseInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            var inputValidationResult = input.GetInputValidation();

            if (!inputValidationResult.IsValid)
                return inputValidationResult;

            var eventId = Guid.NewGuid();

            var @event = new AquisitionCustomerRequestedEvent(
                FirstName: input.FirstName.Value,
                LastName: input.LastName.Value,
                Email: input.Email.Value,
                Phone: input.Phone.Value);

            var eventComposition = new EventBase<AquisitionCustomerRequestedEvent>(
                eventId: eventId, 
                @event: @event, 
                timestamp: DateTime.UtcNow, 
                routingKey: CREATE_AQUISITION_CUSTOMER_ROUTING_KEY, 
                origin: Namespace.Name);

            await _publisher.PublishAsync<AquisitionCustomerRequestedEvent>(
                @event: @eventComposition,
                exchangeName: CREATE_AQUISITION_CUSTOMER_EXCHANGE_NAME,
                routingKey: CREATE_AQUISITION_CUSTOMER_ROUTING_KEY,
                cancellationToken: cancellationToken);

            var successNotification = Notification.Success(
                code: CREATE_AQUISITION_CUSTOMER_REQUEST_SUCCESS_NOTIFICATION_CODE,
                message: CREATE_AQUISITION_CUSTOMER_REQUEST_SUCCESS_NOTIFICATION_MESSAGE);

            return MethodResult.Success([successNotification]);
        }
        catch (Exception ex)
        {
            var errorNotification = Notification.Error(
                code: CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_CODE,
                message: CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_MESSAGE);

            _logger.LogError(
                exception: ex,
                message: "[{Type}] Failed create aquisition customer request use case. Input = {@Input}",
                nameof(CreateAquisitionCustomerRequestUseCase),
                new
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    NotificationCode = CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_CODE,
                    NotificationMessage = CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_MESSAGE
                });

            return MethodResult.Error([errorNotification]);
        }
    }
}
