
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tech.Aquisitions.Customers.Workers.Consumers.Base.ConnectionManager.Interfaces;

namespace Tech.Aquisitions.Customers.Workers.HealthChecks;

public sealed class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnectionManager _connectionManager;
    private readonly ILogger<RabbitMqHealthCheck> _logger;

    public RabbitMqHealthCheck(IRabbitMqConnectionManager connectionManager, ILogger<RabbitMqHealthCheck> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _connectionManager.TryConnectAsync(cancellationToken);

            if (!connection.IsOpen)
            {
                _logger.LogInformation("[{Type}] RabbitMq is unhealthy (connection closed).",
                    nameof(RabbitMqHealthCheck));

                return HealthCheckResult.Unhealthy("RabbitMQ connection is closed.");
            }

            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("[{Type}] RabbitMq is healthy.",
                nameof(RabbitMqHealthCheck));

            return HealthCheckResult.Healthy("RabbitMq connection is healthy.");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("[{Type}] RabbitMq is unhealthy (exception throwed handling health check).",
                    nameof(RabbitMqHealthCheck));

            return HealthCheckResult.Unhealthy("RabbitMq health check failed.", ex);
        }
    }
}
