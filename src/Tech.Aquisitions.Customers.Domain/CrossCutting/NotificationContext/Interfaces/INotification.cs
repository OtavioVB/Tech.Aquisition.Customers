using System.Text.Json.Serialization;
using Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Enums;

namespace Tech.Aquisitions.Customers.Domain.CrossCutting.NotificationContext.Interfaces;

public interface INotification
{
    public string Code { get; }
    public string Message { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TypeNotification Type { get; }
}
