using Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Enums;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;
public readonly struct RequestOriginValueObject
{
    public bool IsValid { get; }

    private readonly RequestOrigin _origin;
    public RequestOrigin Value =>
        IsValid ? _origin : throw new InvalidOperationException("It is not possible to get origin because the value object is invalid.");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult =>
        IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from an invalid value object.");

    private RequestOriginValueObject(
        bool isValid,
        RequestOrigin origin,
        MethodResult methodResult)
    {
        IsValid = isValid;
        _origin = origin;
        _methodResult = methodResult;
    }

    private const string INVALID_ORIGIN_CODE = "INVALID_REQUEST_ORIGIN";
    private const string INVALID_ORIGIN_MESSAGE = "A origem informada é inválida.";

    public static RequestOriginValueObject Build(string? origin)
    {
        if (string.IsNullOrWhiteSpace(origin))
        {
            var notification = Notification.Error(
                code: INVALID_ORIGIN_CODE,
            message: INVALID_ORIGIN_MESSAGE);

            return new RequestOriginValueObject(
            isValid: false,
                origin: default,
                methodResult: MethodResult.Error([notification]));
        }

        if (Enum.TryParse<RequestOrigin>(origin, ignoreCase: true, out var parsed) &&
            Enum.IsDefined(typeof(RequestOrigin), parsed))
        {
            return new RequestOriginValueObject(
                isValid: true,
                origin: parsed,
                methodResult: MethodResult.Success());
        }

        {
            var notification = Notification.Error(
                code: INVALID_ORIGIN_CODE,
                message: INVALID_ORIGIN_MESSAGE);

            return new RequestOriginValueObject(
                isValid: false,
                origin: default,
                methodResult: MethodResult.Error([notification]));
        }
    }

    public static RequestOriginValueObject Build(RequestOrigin origin)
    {
        if (!Enum.IsDefined(typeof(RequestOrigin), origin))
        {
            var notification = Notification.Error(
                code: INVALID_ORIGIN_CODE,
                message: INVALID_ORIGIN_MESSAGE);

            return new RequestOriginValueObject(
                isValid: false,
                origin: default,
                methodResult: MethodResult.Error([notification]));
        }

        return new RequestOriginValueObject(
            isValid: true,
            origin: origin,
            methodResult: MethodResult.Success());
    }
}