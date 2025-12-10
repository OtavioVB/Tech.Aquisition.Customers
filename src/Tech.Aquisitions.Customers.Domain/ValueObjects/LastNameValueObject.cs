using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct LastNameValueObject
{
    public bool IsValid { get; }

    private readonly string? _lastName;
    public string Value => IsValid ? _lastName! : throw new InvalidOperationException("It is not possible to get last name because the value object is invalid");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult => IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from an invalid value object.");

    private LastNameValueObject(bool isValid, MethodResult methodResult, string? lastName = null)
    {
        IsValid = isValid;
        _lastName = lastName;
        _methodResult = methodResult;
    }

    public const int MAX_LENGTH = 128;

    private const string LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE =
        "LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";

    private const string LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE =
        "O sobrenome do cliente não pode ser muito extenso.";

    public static LastNameValueObject Build(string lastName)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (lastName.Length > MAX_LENGTH)
        {
            var errorNotification = Notification.Error(
                code: LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            var errorNotification = Notification.Error(
                code: LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: LAST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (notifications.HasAnyNotifications())
        {
            return new LastNameValueObject(
                isValid: false,
                methodResult: MethodResult.Error(notifications.ToArray()));
        }

        var formatted = lastName.ToUpper();

        return new LastNameValueObject(
            isValid: true,
            lastName: formatted,
            methodResult: MethodResult.Success());
    }

    public static implicit operator LastNameValueObject(string obj)
        => Build(obj);
}

