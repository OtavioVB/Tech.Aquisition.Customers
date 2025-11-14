using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tech.Aquisitions.Customers.Infrascructure.FeatureManager.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.FeatureManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureManagementConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var dictionaryFeatures = configuration.GetRequiredSection("FeatureManagement").Get<IDictionary<string, bool>>()!;

        services.AddSingleton<IFeatureManagement, FeatureManagement>((serviceProvider)
            => new FeatureManagement(
                features: dictionaryFeatures));

        return services;
    }
}
