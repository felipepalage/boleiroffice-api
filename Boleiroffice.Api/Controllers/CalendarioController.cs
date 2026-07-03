using Boleiroffice.Application.DTOs.Calendario;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/calendario")]
[Authorize]
public sealed class CalendarioController : ControllerBase
{
    private readonly ICalendarioService _service;

    public CalendarioController(ICalendarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CalendarioMesResponse>> GetMes(
        [FromQuery] Guid timeId,
        [FromQuery] int ano,
        [FromQuery] int mes,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetMesAsync(timeId, ano, mes, cancellationToken);
        return Ok(result);
    }
}
