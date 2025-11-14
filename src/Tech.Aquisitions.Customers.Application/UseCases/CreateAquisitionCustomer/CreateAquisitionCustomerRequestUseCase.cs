using Microsoft.Extensions.Logging;
using Tech.Aquisitions.Customers.Application.UseCases.Base.Interfaces;
using Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer.Inputs;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer;

public sealed class CreateAquisitionCustomerRequestUseCase : IUseCase<CreateAquisitionCustomerRequestUseCaseInput>
{
    private readonly ILogger<CreateAquisitionCustomerRequestUseCase> _logger;

    public CreateAquisitionCustomerRequestUseCase(ILogger<CreateAquisitionCustomerRequestUseCase> logger)
    {
        _logger = logger;
    }

    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_CODE = "CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR";
    private const string CREATE_AQUISITION_CUSTOMER_REQUEST_ERROR_NOTIFICATION_MESSAGE = "Não foi possível solicitar a criação do cadastro de cliente.";

    public async Task<MethodResult> ExecuteAsync(
        CreateAquisitionCustomerRequestUseCaseInput input, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var inputValidationResult = input.GetInputValidation();

            if (!inputValidationResult.IsValid)
                return inputValidationResult;

            return default;
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
