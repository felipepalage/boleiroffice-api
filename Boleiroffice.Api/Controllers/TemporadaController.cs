using Boleiroffice.Application.DTOs.Temporada;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/temporadas")]
public sealed class TemporadaController : ControllerBase
{
    private readonly ITemporadaService _service;

    public TemporadaController(ITemporadaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TemporadaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TemporadaResponse>> Create(
        [FromBody] TemporadaRequest request, CancellationToken cancellationToken)
    {
        if (!User.HasClaim("is_admin", "true"))
            return Forbid();

        var result = await _service.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/ativar")]
    [Authorize]
    public async Task<ActionResult<TemporadaResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        if (!User.HasClaim("is_admin", "true"))
            return Forbid();

        var result = await _service.ActivateAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!User.HasClaim("is_admin", "true"))
            return Forbid();

        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
