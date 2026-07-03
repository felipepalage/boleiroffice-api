using Boleiroffice.Application.DTOs.Ia;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ia")]
public sealed class IaController : ControllerBase
{
    private readonly IAiService _service;

    public IaController(IAiService service) => _service = service;

    [HttpPost("narracao")]
    public async Task<ActionResult<NarracaoResponse>> Narracao([FromBody] NarracaoRequest request, CancellationToken cancellationToken)
        => Ok(await _service.GerarNarracaoAsync(request, cancellationToken));
}
