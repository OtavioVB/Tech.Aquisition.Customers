using Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Enums;
using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext;

namespace Tech.Aquisitions.Customers.Domain.ValueObjects;

public readonly struct RequestStatusValueObject
{
    public bool IsValid { get; }

    private readonly RequestStatus _status;
    public RequestStatus Value =>
        IsValid ? _status : throw new InvalidOperationException("It is not possible to get status because the value object is invalid.");

    private readonly MethodResult _methodResult;
    public MethodResult MethodResult =>
        IsValid ? _methodResult : throw new InvalidOperationException("It is not possible to get a method result from an invalid value object.");

    private RequestStatusValueObject(
        bool isValid,
        RequestStatus status,
        MethodResult methodResult)
    {
        IsValid = isValid;
        _status = status;
        _methodResult = methodResult;
    }

    private const string INVALID_STATUS_CODE = "INVALID_STATUS";
    private const string INVALID_STATUS_MESSAGE = "O status informado é inválido.";

    public static RequestStatusValueObject Build(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            var notification = Notification.Error(
                code: INVALID_STATUS_CODE,
                message: INVALID_STATUS_MESSAGE);

            return new RequestStatusValueObject(
                isValid: false,
                status: default,
                methodResult: MethodResult.Error([notification]));
        }

        if (Enum.TryParse<RequestStatus>(status, ignoreCase: true, out var parsed) &&
            Enum.IsDefined(typeof(RequestStatus), parsed))
        {
            return new RequestStatusValueObject(
                isValid: true,
                status: parsed,
                methodResult: MethodResult.Success());
        }

        var errorNotification = Notification.Error(
            code: INVALID_STATUS_CODE,
            message: INVALID_STATUS_MESSAGE);

        return new RequestStatusValueObject(
            isValid: false,
            status: default,
            methodResult: MethodResult.Error([errorNotification]));
    }

    public static RequestStatusValueObject Build(RequestStatus status)
    {
        if (!Enum.IsDefined(typeof(RequestStatus), status))
        {
            var notification = Notification.Error(
                code: INVALID_STATUS_CODE,
                message: INVALID_STATUS_MESSAGE);

            return new RequestStatusValueObject(
                isValid: false,
                status: default,
                methodResult: MethodResult.Error([notification]));
        }

        return new RequestStatusValueObject(
            isValid: true,
            status: status,
            methodResult: MethodResult.Success());
    }
}