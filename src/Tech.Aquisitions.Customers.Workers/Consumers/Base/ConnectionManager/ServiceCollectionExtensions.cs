using Tech.Aquisitions.Customers.Workers.Consumers.Base.Configuration;
using Tech.Aquisitions.Customers.Workers.Consumers.Base.ConnectionManager.Interfaces;

namespace Tech.Aquisitions.Customers.Workers.Consumers.Base.ConnectionManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqConnectionManagerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConfiguration = configuration.GetRequiredSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>()!;

        services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>((serviceProvider)
            => new RabbitMqConnectionManager(
                logger: serviceProvider.GetRequiredService<ILogger<RabbitMqConnectionManager>>(),
                configuration: rabbitMqConfiguration));

        return services;
    }
}
