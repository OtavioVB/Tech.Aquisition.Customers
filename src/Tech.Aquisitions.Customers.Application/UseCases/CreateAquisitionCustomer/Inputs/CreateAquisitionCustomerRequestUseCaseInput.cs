using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;
using Tech.Aquisitions.Customers.Domain.ValueObjects;

namespace Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer.Inputs;

public sealed record CreateAquisitionCustomerRequestUseCaseInput
{
    public FirstNameValueObject FirstName { get; set; }
    public LastNameValueObject LastName { get; set; }
    public EmailValueObject Email { get; set; }
    public PhoneValueObject Phone { get; set; }

    private CreateAquisitionCustomerRequestUseCaseInput(FirstNameValueObject firstName, LastNameValueObject lastName, EmailValueObject email, PhoneValueObject phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }

    public static CreateAquisitionCustomerRequestUseCaseInput Create(FirstNameValueObject firstName, LastNameValueObject lastName, EmailValueObject email, PhoneValueObject phone)
        => new(firstName, lastName, email, phone);

    public MethodResult GetInputValidation()
        => MethodResult.Factory(FirstName.MethodResult, LastName.MethodResult, Email.MethodResult, Phone.MethodResult);
}
