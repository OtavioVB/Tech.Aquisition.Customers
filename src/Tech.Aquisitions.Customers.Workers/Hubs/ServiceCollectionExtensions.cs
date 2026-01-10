using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Tech.Aquisitions.Customers.Workers.Hubs.CustomerAquisitionContext;

namespace Tech.Aquisitions.Customers.Workers.Hubs;

public static class ServiceCollectionExtensions
{
    public static IEndpointRouteBuilder MapHubsConfiguration(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapHub<CustomerAquisitionHub>(CustomerAquisitionHub.Path);

        return endpoint;
    }
}
