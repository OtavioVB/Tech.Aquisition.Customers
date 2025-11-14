using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Net.Security;
using System.Security.Authentication;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Configuration;
using Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.ConnectionManager;

public sealed class RabbitMqConnectionManager : IRabbitMqConnectionManager
{
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly RabbitMqConfiguration _configuration;

    public RabbitMqConnectionManager(ILogger<RabbitMqConnectionManager> logger, RabbitMqConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private IConnection? _connection;


    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(_connectionMaxAccessLockCount);
    private const int _connectionMaxAccessLockCount = 1;

    public async Task<IConnection> TryConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_connection != null)
            return _connection;

        await _connectionLock.WaitAsync(cancellationToken);

        var connectionFactory = new ConnectionFactory()
        {
            HostName = _configuration.HostName,
            Port = _configuration.Port,
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            VirtualHost = _configuration.VirtualHost,
            ClientProvidedName = _configuration.ClientProviderName,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(1),
            Ssl =
            {
                Enabled = true,
                ServerName = _configuration.HostName,
                Version = SslProtocols.Tls12,
                AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateNotAvailable
            }
        };

        var delay = TimeSpan.FromMilliseconds(_configuration.RetryConnectionDelayInMs);

        try
        {
            _logger.LogInformation("[{Type}] Trying to create singleton rabbitmq client connection. Input = {@Input}",
                nameof(RabbitMqConnectionManager),
                new
                {
                    _configuration.ClientProviderName,
                    _configuration.RetryConnectionDelayInMs,
                });

            _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

            _logger.LogInformation("[{Type}] Rabbitmq client connection successfull created. Input = {@Input}",
                nameof(RabbitMqConnectionManager),
                new
                {
                    _configuration.ClientProviderName,
                    _configuration.RetryConnectionDelayInMs,
                });

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<IConnection> GetResilientAlwaysOpennedConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await TryConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(
                ex,
                "[{Type}] Failed to connect to RabbitMQ. Retrying in {Delay}ms.",
                nameof(RabbitMqConnectionManager),
                _configuration.RetryConnectionDelayInMs);

            await Task.Delay(TimeSpan.FromMilliseconds(_configuration.RetryConnectionDelayInMs), cancellationToken);

            return await GetResilientAlwaysOpennedConnectionAsync(cancellationToken);
        }
    }
}
