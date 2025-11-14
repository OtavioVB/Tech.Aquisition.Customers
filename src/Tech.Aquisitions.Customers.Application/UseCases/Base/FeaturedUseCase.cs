using Microsoft.Extensions.Logging;
using Tech.Aquisitions.Customers.Application.UseCases.Base.Interfaces;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;
using Tech.Aquisitions.Customers.Infrascructure.FeatureManager.Interfaces;

namespace Tech.Aquisitions.Customers.Application.UseCases.Base;

public abstract class FeaturedUseCase<TInput> : IUseCase<TInput>
{
    protected readonly ILogger<FeaturedUseCase<TInput>> _logger;
    protected readonly IFeatureManagement _featureManagement;

    protected abstract string FeatureName { get; }

    private const string FEATURE_IS_NOT_AVAILABLE_NOTIFICATION_CODE = "FEATURE_IS_NOT_AVAILABLE";
    private const string FEATURE_IS_NOT_AVAILABLE_NOTIFICATION_MESSAGE = "A funcionalidade não está habilitada para seu usuário.";

    protected FeaturedUseCase(ILogger<FeaturedUseCase<TInput>> logger, IFeatureManagement featureManagement)
    {
        _logger = logger;
        _featureManagement = featureManagement;
    }

    protected virtual Task<bool> CanHandleAsync(TInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            var isFeatureAvailable = _featureManagement.IsFeatureAvailable(FeatureName);

            return Task.FromResult(isFeatureAvailable);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                exception: ex,
                message: "[{Type}] An error has been throwed handling can handle operation on featured use case. Input = {@Input}",
                nameof(FeaturedUseCase<TInput>),
                new
                {
                    CanHandle = false,
                });

            return Task.FromResult(false);
        }
    }

    public async Task<MethodResult> ExecuteAsync(TInput input, CancellationToken cancellationToken = default)
    {
        var canHandle = await CanHandleAsync(input, cancellationToken);

        if (canHandle)
            return await HandleAsync(input, cancellationToken);

        var featureNotAvailableNotification = Notification.Error(
            code: FEATURE_IS_NOT_AVAILABLE_NOTIFICATION_CODE,
            message: FEATURE_IS_NOT_AVAILABLE_NOTIFICATION_MESSAGE);

        return MethodResult.Error([featureNotAvailableNotification]);
    }

    protected abstract Task<MethodResult> HandleAsync(TInput input, CancellationToken cancellationToken = default);
}
