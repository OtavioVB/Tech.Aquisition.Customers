using Microsoft.AspNetCore.SignalR;

namespace Tech.Aquisitions.Customers.Workers.Hubs.CustomerAquisitionContext;

public sealed class CustomerAquisitionHub : Hub
{
    private readonly ILogger<CustomerAquisitionHub> _logger;

    public CustomerAquisitionHub(ILogger<CustomerAquisitionHub> logger)
    {
        _logger = logger;
    }

    public const string Path = "/hubs/aquisitions/customers";

    public async Task CreateCustomerAquisitionRequestedNotification(string request)
    {
        var groupName = CustomerAquisitionHubSpecifications.GetUserUniqueChannelGroup(Context.Items["Id"]!.ToString()!);

        await Clients.Group(groupName).SendAsync("CreateCustomerAquisitionRequestedNotification", request);
    }

    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        var authorization = httpContext?.Request.Headers.Authorization.FirstOrDefault();

        if (authorization == null)
        {
            _logger.LogWarning(
                message: "[{Type}] User has been not connected to SignalR, because is not possible to get the authorization. ",
                nameof(CustomerAquisitionHub));

            Context.Abort();

            return;
        }

        var userId = await OperationBusinessNotImplementedAsync(authorization, Context.ConnectionAborted);

        Context.Items.Add("Id", userId);

        var userUniqueChannelGroup = CustomerAquisitionHubSpecifications.GetUserUniqueChannelGroup(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, userUniqueChannelGroup, Context.ConnectionAborted);

        _logger.LogInformation(
            "[{Type}] User has been connected to SignalR. Input = {@Input}",
            nameof(CustomerAquisitionHub),
            new 
            {
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                UserUniqueChannelGroup = userUniqueChannelGroup
            });

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation(
            exception: exception,
            message: "[{Type}] The user has been disconnected from SignalR. Input = {@Input}",
            nameof(CustomerAquisitionHub),
            new 
            {
                ConnectionId = Context.ConnectionId,
            });

        return base.OnDisconnectedAsync(exception);
    }

    private Task<string> OperationBusinessNotImplementedAsync(string authorization, CancellationToken cancellationToken = default)
        => Task.FromResult(Guid.NewGuid().ToString());
}
