using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct PhoneValueObject
{
    public bool IsValid { get; }

    private readonly string? _phone;
    public string Value => IsValid ? _phone! : throw new InvalidOperationException("It is not possible to get phone because value object is invalid");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult => IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from invalid value object.");

    private PhoneValueObject(bool isValid, MethodResult methodResult, string? phone = null)
    {
        IsValid = isValid;
        _phone = phone;
        _methodResult = methodResult;
    }

    public const int REQUIRED_LENGTH = 11;

    private const string PHONE_EMPTY_CODE = "PHONE_EMPTY";
    private const string PHONE_EMPTY_MESSAGE = "O telefone não pode ser vazio.";

    private const string PHONE_INVALID_LENGTH_CODE = "PHONE_INVALID_LENGTH";
    private const string PHONE_INVALID_LENGTH_MESSAGE = "O telefone deve conter exatamente 11 dígitos.";

    private const string PHONE_INVALID_FORMAT_CODE = "PHONE_INVALID_FORMAT";
    private const string PHONE_INVALID_FORMAT_MESSAGE = "O telefone informado possui um formato inválido.";

    public static PhoneValueObject Build(string phone)
    {
        const int MAX_POSSIBLE_NOTIFICATIONS = 3;
        var notifications = new List<INotification>(MAX_POSSIBLE_NOTIFICATIONS);

        if (string.IsNullOrWhiteSpace(phone))
        {
            notifications.Add(Notification.Error(
                code: PHONE_EMPTY_CODE,
                message: PHONE_EMPTY_MESSAGE));
        }

        var digitsOnly = ExtractDigits(phone);

        if (digitsOnly.Length != REQUIRED_LENGTH)
        {
            notifications.Add(Notification.Error(
                code: PHONE_INVALID_LENGTH_CODE,
                message: PHONE_INVALID_LENGTH_MESSAGE));
        }

        if (!AllCharactersAreDigits(digitsOnly) || digitsOnly.Length != REQUIRED_LENGTH)
        {
            notifications.Add(Notification.Error(
                code: PHONE_INVALID_FORMAT_CODE,
                message: PHONE_INVALID_FORMAT_MESSAGE));
        }

        if (notifications.HasAnyNotifications())
        {
            return new PhoneValueObject(
                isValid: false,
                methodResult: MethodResult.Error(notifications.ToArray()));
        }

        return new PhoneValueObject(
            isValid: true,
            phone: digitsOnly,
            methodResult: MethodResult.Success());
    }

    private static string ExtractDigits(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var buffer = new char[input.Length];
        var index = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c >= '0' && c <= '9')
            {
                buffer[index++] = c;
            }
        }

        return new string(buffer, 0, index);
    }

    private static bool AllCharactersAreDigits(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c < '0' || c > '9')
                return false;
        }
        return true;
    }

    public static implicit operator PhoneValueObject(string obj)
        => Build(obj);
}