namespace Tech.Aquisitions.Customers.WebApi.Controllers.Payloads;

public sealed record CreateAquisitionCustomerV1Request(string FirstName, string LastName, string Phone, string Email);