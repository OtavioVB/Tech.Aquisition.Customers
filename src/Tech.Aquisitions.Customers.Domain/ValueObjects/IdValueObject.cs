using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct IdValueObject
{
    public bool IsValid { get; }

    private readonly Guid _id;
    public Guid Value => IsValid
        ? _id
        : throw new InvalidOperationException("It is not possible to get Id because value object is invalid");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult => IsValid
        ? _methodResult
        : throw new InvalidOperationException("It is not possible to get a method result from an invalid value object.");

    private IdValueObject(bool isValid, Guid id, MethodResult methodResult)
    {
        IsValid = isValid;
        _id = id;
        _methodResult = methodResult;
    }

    private const string INVALID_ID_CODE = "INVALID_ID";
    private const string INVALID_ID_MESSAGE = "O ID informado não possui um formato válido.";

    public static IdValueObject Build(string? id = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            var generated = Guid.NewGuid();

            return new IdValueObject(
                isValid: true,
                id: generated,
                methodResult: MethodResult.Success());
        }

        if (Guid.TryParse(id, out var parsed))
        {
            return new IdValueObject(
                isValid: true,
                id: parsed,
                methodResult: MethodResult.Success());
        }

        var notification = Notification.Error(
            code: INVALID_ID_CODE,
            message: INVALID_ID_MESSAGE);

        return new IdValueObject(
            isValid: false,
            id: Guid.Empty,
            methodResult: MethodResult.Error([notification]));
    }
}
