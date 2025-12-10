
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tech.Aquisitions.Customers.Application.UseCases;
using Tech.Aquisitions.Customers.Infrascructure.FeatureManager;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Publisher;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.HealthChecks;

namespace Tech.Aquisitions.Customers.WebApi;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services
            .AddRabbitMqConnectionManagerConfiguration(builder.Configuration)
            .AddRabbitMqPublisherConfiguration(builder.Configuration)
            .AddFeatureManagementConfiguration(builder.Configuration);

        builder.Services
            .AddUseCasesConfiguration(builder.Configuration);

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
