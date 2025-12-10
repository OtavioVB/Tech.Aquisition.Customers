using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tech.Aquisitions.Customers.Application.UseCases.Base.Interfaces;
using Tech.Aquisitions.Customers.Application.UseCases.CreateAquisitionCustomer.Inputs;
using Tech.Aquisitions.Customers.WebApi.Controllers.Payloads;

namespace Tech.Aquisitions.Customers.WebApi.Controllers;

[Route("api/v1/aquisitions")]
[ApiController]
public sealed class AquisitionsV1Controller : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAquisitionCustomerRequestAsync(
        [FromServices] IUseCase<CreateAquisitionCustomerRequestUseCaseInput> useCase,
        [FromHeader(Name = "X-Origin-Service")] string originService,
        [FromHeader(Name = "X-Correlation-Id")] Guid correlationId,
        [FromBody] CreateAquisitionCustomerV1Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(
            input: CreateAquisitionCustomerRequestUseCaseInput.Create(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                phone: request.Phone),
            cancellationToken: cancellationToken);

        if (result.IsError)
            return BadRequest(result.Notifications);

        return Ok(result.Notifications);
    }
}