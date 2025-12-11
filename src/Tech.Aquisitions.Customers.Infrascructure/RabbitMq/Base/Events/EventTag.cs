namespace Tech.Aquisitions.Customers.Infrascructure.RabbitMq.Base.Events;

public static class EventTag
{
    /* ALL EVENTS */
    public const string Id = "X-Event-Id";
    public const string Timestamp = "X-Event-Timestamp";
    public const string Origin = "X-Origin-Service";
    public const string RoutingKey = "X-Routing-Key";
    public const string Exchange = "X-Exchange";

    public const string Retry = "X-Retry-Count";

    /* DLQ */
    public const string ErrorMessage = "X-Error-Message";
    public const string StackTrace = "X-Stack-Trace";
    public const string DlqPublishedTimestamp = "X-Dlq-Published-Timestamp";
    public const string DeadRoutingKey = "X-Dead-Routing-Key";
    public const string DeadExchange = "X-Dead-Exchange";
}
