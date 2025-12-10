using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct FirstNameValueObject
{
    public bool IsValid { get; }

    private readonly string? _firstName;
    public string Value => IsValid ? _firstName! : throw new InvalidOperationException("Is not possible to get first name because value object is invalid");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult => IsValid ? _methodResult : throw new InvalidOperationException("Is not possible to get a method result from invalid value object.");

    private FirstNameValueObject(bool isValid, MethodResult methodResult, string? firstName = null)
    {
        IsValid = isValid;
        _firstName = firstName;
        _methodResult = methodResult;
    }

    public const int MAX_LENGTH = 64;

    private const string FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE = "FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED";
    private const string FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE = "O nome do cliente não pode ser muito extenso.";

    public static FirstNameValueObject Build(string firstName)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 2;

        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (firstName.Length > MAX_LENGTH)
        {
            var errorNotification = Notification.Error(
                code: FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            var errorNotification = Notification.Error(
                code: FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_CODE,
                message: FIRST_NAME_LENGTH_COULD_NOT_BE_GREATER_THAN_THE_MAXIMUM_ALLOWED_NOTIFICATION_MESSAGE);

            notifications.Add(errorNotification);
        }

        if (notifications.HasAnyNotifications())
        {
            return new FirstNameValueObject(
                isValid: false,
                methodResult: MethodResult.Error(notifications.ToArray()));
        }

        var formatted = firstName.ToUpper();

        return new FirstNameValueObject(
            isValid: true,
            firstName: formatted,
            methodResult: MethodResult.Success());
    }

    public static implicit operator FirstNameValueObject(string obj)
        => Build(obj);
}
