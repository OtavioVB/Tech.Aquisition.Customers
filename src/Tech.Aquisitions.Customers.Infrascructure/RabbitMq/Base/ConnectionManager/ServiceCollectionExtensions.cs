using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Configuration;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager;

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
