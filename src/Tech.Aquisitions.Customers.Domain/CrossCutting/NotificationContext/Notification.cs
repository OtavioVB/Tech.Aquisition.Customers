using System.Collections.ObjectModel;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Enums;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;

namespace Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

public readonly struct Notification : INotification
{
    private Notification(string code, string message, TypeNotification type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }
    public string Message { get; }
    public TypeNotification Type { get; }

    private const int _codeMaxLength = 124;
    private const int _messageMaxLength = 256;

    public static Notification Build(string code, string message, TypeNotification type)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length > _codeMaxLength)
            throw new ArgumentException(nameof(Code), "The notification code must be valid.");

        if (string.IsNullOrWhiteSpace(message) || message.Length > _messageMaxLength)
            throw new ArgumentException(nameof(Message), "The notification message must be valid.");

        if (!Enum.IsDefined(type))
            throw new ArgumentException(nameof(Type), "The notification type must be valid.");

        return new(code, message, type);
    }

    public static Notification Success(string code, string message)
        => Build(code, message, TypeNotification.Success);

    public static Notification Information(string code, string message)
        => Build(code, message, TypeNotification.Information);

    public static Notification Error(string code, string message)
        => Build(code, message, TypeNotification.Error);
}

public static class NotificationExtensions
{
    public static bool HasAnyNotifications(this INotification[] notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this IList<INotification> notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this IEnumerable<INotification> notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this List<INotification> notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this ReadOnlyCollection<INotification> notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this Collection<INotification> notifications)
        => notifications.Any();
    public static bool HasAnyNotifications(this IReadOnlyList<INotification> notifications)
        => notifications.Any();
}
