using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tech.Aquisitions.Customers.Workers.Consumers;
using Tech.Aquisitions.Customers.Workers.HealthChecks;

namespace Tech.Aquisitions.Customers.Workers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddJsonConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddAquisitionCustomerRequestedConsumerConfiguration(context.Configuration);

                    services
                        .AddHealthChecks()
                        .AddCheck<RabbitMqHealthCheck>(
                            name: nameof(RabbitMqHealthCheck),
                            failureStatus: HealthStatus.Unhealthy,
                            tags: ["ready", "rabbitmq"]);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenAnyIP(8080);
                    });

                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                            {
                                Predicate = r => r.Tags.Contains("ready")
                            });

                            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                            {
                                Predicate = _ => false
                            });
                        });
                    });
                });

            var host = builder.Build();

            await host.RunAsync();
        }
    }
}