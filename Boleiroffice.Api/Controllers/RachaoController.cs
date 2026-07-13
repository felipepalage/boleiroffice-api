using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Rachao;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/rachao")]
public sealed class RachaoController : ControllerBase
{
    private readonly IRachaoEventoService _service;

    public RachaoController(IRachaoEventoService service) => _service = service;

    // ---------- Dono do rachão (autenticado) ----------

    [Authorize]
    [HttpPost("eventos")]
    public async Task<ActionResult<RachaoEventoResponse>> Criar([FromBody] CriarRachaoRequest request, CancellationToken cancellationToken)
        => Ok(await _service.CriarAsync(User.GetCurrentUser().EmpresaId, request, cancellationToken));

    [Authorize]
    [HttpGet("eventos/ativo")]
    public async Task<ActionResult<RachaoEventoResponse>> Ativo(CancellationToken cancellationToken)
    {
        var evento = await _service.GetAtivoAsync(User.GetCurrentUser().EmpresaId, cancellationToken);
        return evento is null ? NoContent() : Ok(evento);
    }

    // ---------- Link público (sem login) ----------

    [AllowAnonymous]
    [HttpGet("publico/{token}")]
    public async Task<ActionResult<RachaoPublicoResponse>> Publico(string token, CancellationToken cancellationToken)
    {
        var evento = await _service.GetPublicoAsync(token, cancellationToken);
        return evento is null ? NotFound() : Ok(evento);
    }

    [AllowAnonymous]
    [HttpPost("publico/{token}/confirmar")]
    public async Task<ActionResult<RachaoPublicoResponse>> Confirmar(string token, [FromBody] ConfirmarPresencaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _service.ConfirmarAsync(token, request, cancellationToken);
        return evento is null ? NotFound() : Ok(evento);
    }

    [AllowAnonymous]
    [HttpPost("publico/{token}/desistir")]
    public async Task<ActionResult<RachaoPublicoResponse>> Desistir(string token, [FromBody] DesistirPresencaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _service.DesistirAsync(token, request, cancellationToken);
        return evento is null ? NotFound() : Ok(evento);
    }
}
