using System.Net.Mail;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct EmailValueObject
{
    public bool IsValid { get; }

    private readonly string? _email;
    public string Value => IsValid ? _email! : throw new InvalidOperationException("It is not possible to get email because value object is invalid");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult => IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from invalid value object.");

    private EmailValueObject(bool isValid, MethodResult methodResult, string? email = null)
    {
        IsValid = isValid;
        _email = email;
        _methodResult = methodResult;
    }

    public const int MAX_LENGTH = 256;

    private const string EMAIL_COULD_NOT_BE_EMPTY_NOTIFICATION_CODE = "EMAIL_COULD_NOT_BE_EMPTY";
    private const string EMAIL_COULD_NOT_BE_EMPTY_NOTIFICATION_MESSAGE = "O email não pode ser vazio.";

    private const string EMAIL_LENGTH_INVALID_NOTIFICATION_CODE = "EMAIL_LENGTH_INVALID";
    private const string EMAIL_LENGTH_INVALID_NOTIFICATION_MESSAGE = "O email excede o tamanho máximo permitido.";

    private const string EMAIL_FORMAT_INVALID_NOTIFICATION_CODE = "EMAIL_FORMAT_INVALID";
    private const string EMAIL_FORMAT_INVALID_NOTIFICATION_MESSAGE = "O email informado não possui um formato válido.";

    public static EmailValueObject Build(string email)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 3;
        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (string.IsNullOrWhiteSpace(email))
        {
            notifications.Add(Notification.Error(
                code: EMAIL_COULD_NOT_BE_EMPTY_NOTIFICATION_CODE,
                message: EMAIL_COULD_NOT_BE_EMPTY_NOTIFICATION_MESSAGE));
        }

        if (email.Length > MAX_LENGTH)
        {
            notifications.Add(Notification.Error(
                code: EMAIL_LENGTH_INVALID_NOTIFICATION_CODE,
                message: EMAIL_LENGTH_INVALID_NOTIFICATION_MESSAGE));
        }

        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            notifications.Add(Notification.Error(
                code: EMAIL_FORMAT_INVALID_NOTIFICATION_CODE,
                message: EMAIL_FORMAT_INVALID_NOTIFICATION_MESSAGE));
        }

        if (notifications.HasAnyNotifications())
        {
            return new EmailValueObject(
                isValid: false,
                methodResult: MethodResult.Error(notifications.ToArray()));
        }

        var normalized = email.Trim().ToLowerInvariant();

        return new EmailValueObject(
            isValid: true,
            email: normalized,
            methodResult: MethodResult.Success());
    }

    public static implicit operator EmailValueObject(string obj)
        => Build(obj);
}