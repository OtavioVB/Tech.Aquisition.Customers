using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqPublisherConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        return services;
    }
}
