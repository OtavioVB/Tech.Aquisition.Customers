using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tech.Aquisitions.Customers.WebApi.Controllers.Payloads;

namespace Tech.Aquisitions.Customers.WebApi.Controllers;

[Route("api/v1/aquisitions")]
[ApiController]
public sealed class AquisitionsV1Controller : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAquisitionCustomerRequestAsync(
        [FromHeader(Name = "X-Origin-Service")] string originService,
        [FromHeader(Name = "X-Correlation-Id")] Guid correlationId,
        [FromBody] CreateAquisitionCustomerV1Request request,
        CancellationToken cancellationToken = default)
    {
        return NoContent();
    }
}
