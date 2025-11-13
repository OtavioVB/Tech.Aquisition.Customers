namespace Tech.Aquisitions.Customers.Workers.Consumers;

public sealed record AquisitionCustomerRequestedEvent(Guid EventId, string FirstName, string LastName, string Email, string Phone);
