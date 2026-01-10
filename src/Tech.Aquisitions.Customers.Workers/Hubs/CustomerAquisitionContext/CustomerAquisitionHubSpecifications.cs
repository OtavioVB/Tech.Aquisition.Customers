namespace Tech.Aquisitions.Customers.Workers.Hubs.CustomerAquisitionContext;

public static class CustomerAquisitionHubSpecifications
{
    public static string GetUserUniqueChannelGroup(string userId)
        => $"user-{userId}";
}