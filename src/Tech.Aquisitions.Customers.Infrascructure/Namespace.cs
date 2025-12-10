namespace Tech.Aquisitions.Customers.Infrascructure;

public static class Namespace
{
    public static string Name => GetOriginName();

    private static string GetOriginName()
        => Environment.GetEnvironmentVariable("NAMESPACE") ?? throw new ArgumentNullException("The 'NAMESPACE' of application is not defined.");
}
