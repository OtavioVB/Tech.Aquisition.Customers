using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct DateTimeValueObject
{
    public bool IsValid { get; }

    private readonly DateTime _dateTime;
    public DateTime Value =>
        IsValid ? _dateTime : throw new InvalidOperationException("It is not possible to get DateTime because value object is invalid.");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult =>
        IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from an invalid value object.");

    private DateTimeValueObject(
        bool isValid,
        DateTime value,
        MethodResult methodResult)
    {
        IsValid = isValid;
        _dateTime = value;
        _methodResult = methodResult;
    }

    private const string INVALID_DATE_CODE = "INVALID_DATE";
    private const string INVALID_DATE_MESSAGE = "A data informada é inválida.";

    public static DateTimeValueObject Build(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            var notification = Notification.Error(INVALID_DATE_CODE, INVALID_DATE_MESSAGE);
            return new DateTimeValueObject(false, default, MethodResult.Error([notification]));
        }

        if (DateTime.TryParse(raw, out var parsed))
        {
            return new DateTimeValueObject(true, parsed, MethodResult.Success());
        }

        var errorNotification = Notification.Error(INVALID_DATE_CODE, INVALID_DATE_MESSAGE);
        return new DateTimeValueObject(false, default, MethodResult.Error([errorNotification]));
    }

    public static DateTimeValueObject Build(DateTime dateTime)
    {
        return new DateTimeValueObject(
            isValid: true,
            value: dateTime,
            methodResult: MethodResult.Success());
    }
}
