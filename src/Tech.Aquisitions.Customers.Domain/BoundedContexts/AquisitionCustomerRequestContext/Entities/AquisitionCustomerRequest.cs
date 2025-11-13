using Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Enums;

namespace Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Entities;

public sealed record AquisitionCustomerRequest
{
    public AquisitionCustomerRequest(string firstName, string lastName, DateTime createdAt, Guid createdCorrelationId, AquisitionCustomerRequestStatus status, DateTime lastModifiedAt, Guid lastModifiedCorrelationId)
    {
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = createdAt;
        CreatedCorrelationId = createdCorrelationId;
        Status = status;
        LastModifiedAt = lastModifiedAt;
        LastModifiedCorrelationId = lastModifiedCorrelationId;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedCorrelationId { get; set; }
    public AquisitionCustomerRequestStatus Status { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public Guid LastModifiedCorrelationId { get; set; }
}
