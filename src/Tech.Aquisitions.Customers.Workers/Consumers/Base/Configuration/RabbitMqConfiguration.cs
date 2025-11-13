using System.Security.Authentication;

namespace Tech.Aquisitions.Customers.Workers.Consumers.Base.Configuration;

public sealed record RabbitMqConfiguration(
    string HostName,
    string UserName,
    string Password,
    int Port,
    string VirtualHost,
    string ClientProviderName,
    int RetryConnectionDelayInMs,
    SslProtocols SslProtocol);
