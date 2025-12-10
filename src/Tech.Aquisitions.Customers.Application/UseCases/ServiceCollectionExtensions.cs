using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tech.Aquisitions.Customers.Application.UseCases.Base.Interfaces;
using Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer;
using Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer.Inputs;

namespace Tech.Aquisitions.Customers.Application.UseCases;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCasesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUseCase<CreateAquisitionCustomerRequestUseCaseInput>, CreateAquisitionCustomerRequestUseCase>();

        return services;
    }
}
