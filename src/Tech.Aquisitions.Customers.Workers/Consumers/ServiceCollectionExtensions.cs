namespace Tech.Aquisitions.Customers.Workers.Consumers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAquisitionCustomerRequestedConsumerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<AquisitionCustomerRequestedConsumer>();

        return services;
    }
}
