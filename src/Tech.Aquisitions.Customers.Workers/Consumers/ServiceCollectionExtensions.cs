using Tech.Aquisitions.Customers.Workers.Consumers.Base.ConnectionManager;

namespace Tech.Aquisitions.Customers.Workers.Consumers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAquisitionCustomerRequestedConsumerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMqConnectionManagerConfiguration(configuration);

        services.AddHostedService<AquisitionCustomerRequestedConsumer>();

        return services;
    }
}
