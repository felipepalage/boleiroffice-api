using Boleiroffice.Application.DTOs.Relatorio;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
public sealed class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _service;

    public RelatorioController(IRelatorioService service)
    {
        _service = service;
    }

    [HttpGet("{timeId:guid}")]
    public async Task<ActionResult<RelatorioTimeResponse>> GetRelatorio(
        Guid timeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRelatorioAsync(timeId, cancellationToken);
        return Ok(result);
    }
}
