using Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Enums;

namespace Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Entities;

public sealed record AquisitionCustomerRequest
{
    public AquisitionCustomerRequest(
        string firstName, 
        string lastName, 
        string email,
        string phone,
        DateTime createdAt, 
        Guid createdCorrelationId, 
        AquisitionCustomerRequestStatus status,
        AquisitionCustomerRequestedOrigin origin,
        DateTime lastModifiedAt, 
        Guid lastModifiedCorrelationId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        CreatedAt = createdAt;
        CreatedCorrelationId = createdCorrelationId;
        Status = status;
        Origin = origin;
        LastModifiedAt = lastModifiedAt;
        LastModifiedCorrelationId = lastModifiedCorrelationId;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedCorrelationId { get; set; }
    public AquisitionCustomerRequestStatus Status { get; set; }
    public AquisitionCustomerRequestedOrigin Origin { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public Guid LastModifiedCorrelationId { get; set; }
}
