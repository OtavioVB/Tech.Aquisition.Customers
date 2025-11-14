using Tech.Aquisitions.Customers.Domain.ValueObjects;

namespace Tech.Aquisitions.Customers.Domain.BoundedContexts.AquisitionCustomerRequestContext.Entities;

public sealed record AquisitionCustomerRequest
{
    public AquisitionCustomerRequest(FirstNameValueObject firstName, LastNameValueObject lastName, EmailValueObject email, PhoneValueObject phone, DateTimeValueObject createdAt, IdValueObject createdCorrelationId, RequestStatusValueObject status, RequestOriginValueObject origin, DateTimeValueObject lastModifiedAt, IdValueObject lastModifiedCorrelationId)
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

    public FirstNameValueObject FirstName { get; set; }
    public LastNameValueObject LastName { get; set; }
    public EmailValueObject Email { get; set; }
    public PhoneValueObject Phone { get; set; }
    public DateTimeValueObject CreatedAt { get; set; }
    public IdValueObject CreatedCorrelationId { get; set; }
    public RequestStatusValueObject Status { get; set; }
    public RequestOriginValueObject Origin { get; set; }
    public DateTimeValueObject LastModifiedAt { get; set; }
    public IdValueObject LastModifiedCorrelationId { get; set; }
}
