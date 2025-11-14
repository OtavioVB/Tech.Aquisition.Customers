using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext.Enums;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;

namespace Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;

public readonly struct MethodResult
{
    public bool IsValid { get; }
    public bool IsSuccess => IsValid ? (Type == EnumTypeResult.Success) : false;
    public bool IsError => !IsSuccess;
    public EnumTypeResult Type { get; }
    public INotification[] Notifications { get; }

    private MethodResult(EnumTypeResult type, INotification[] notifications)
    {
        IsValid = true;
        Type = type;
        Notifications = notifications;
    }
    public static MethodResult Error(INotification[]? notifications = null)
    => Factory(EnumTypeResult.Error, notifications);
    public static MethodResult Success(INotification[]? notifications = null)
    => Factory(EnumTypeResult.Success, notifications);
    public static MethodResult Factory(EnumTypeResult type, INotification[]? notifications = null)
        => new MethodResult(type, notifications ?? []);

    public static MethodResult Factory(params MethodResult[] processResults)
    {

        var totalNotifications = 0;
        var totalExceptions = 0;

        for (int i = 0; i < processResults.Length; i++)
        {
            totalNotifications += processResults[i].Notifications?.Length ?? 0;
            totalExceptions += processResults[i].Notifications?.Length ?? 0;
        }

        EnumTypeResult? newTypeProcessResult = null;

        INotification[]? newMessageArray = null;
        newMessageArray = new INotification[totalNotifications];

        var lastMessageIndex = 0;

        for (int i = 0; i < processResults.Length; i++)
        {
            var processResult = processResults[i];

            if (newTypeProcessResult is null)
                newTypeProcessResult = processResult.Type;
            else if (newTypeProcessResult == EnumTypeResult.Success && processResult.Type != EnumTypeResult.Success)
                newTypeProcessResult = processResult.Type;

            if (processResult.Notifications is not null)
            {
                Array.Copy(
                    sourceArray: processResult.Notifications,
                    sourceIndex: 0,
                    destinationArray: newMessageArray!,
                    destinationIndex: lastMessageIndex,
                    length: processResult.Notifications.Length
                );

                lastMessageIndex += processResult.Notifications.Length;
            }
        }

        return new MethodResult(newTypeProcessResult!.Value, newMessageArray);
    }
}

public readonly struct MethodResult<TOutput>
{
    public bool IsValid { get; }
    public bool IsSuccess => IsValid ? (Type == EnumTypeResult.Success) : false;
    public bool IsError => !IsSuccess;
    public EnumTypeResult Type { get; }
    public INotification[] Notifications { get; }
    public TOutput? Output { get; }

    private MethodResult(EnumTypeResult type, INotification[] notifications, TOutput? output = default)
    {
        IsValid = true;
        Type = type;
        Notifications = notifications;
        Output = output;
    }
    public static MethodResult<TOutput> Error(INotification[]? notifications = null, TOutput? output = default)
    => Factory(EnumTypeResult.Error, notifications);
    public static MethodResult<TOutput> Success(INotification[]? notifications = null, TOutput? output = default)
    => Factory(EnumTypeResult.Success, notifications);
    public static MethodResult<TOutput> Factory(EnumTypeResult type, INotification[]? notifications = null, TOutput? output = default)
        => new MethodResult<TOutput>(type, notifications ?? [], output);

    public static MethodResult<TOutput> Factory(
        TOutput? output = default,
        params MethodResult[] methodResults)
    {
        var methodResult = MethodResult.Factory(methodResults);

        return new MethodResult<TOutput>(methodResult.Type, methodResult.Notifications, output);
    }
}