namespace Tech.Aquisitions.Customers.Workers.Consumers;

public sealed record AquisitionCustomerRequestedEvent(string FirstName, string LastName, string Email, string Phone);
