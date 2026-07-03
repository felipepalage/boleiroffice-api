using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Notificacoes;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/notificacoes")]
[Authorize]
public sealed class NotificacoesController : ControllerBase
{
    private readonly INotificacaoService _service;

    public NotificacoesController(INotificacaoService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<NotificacoesResumoResponse>> GetAll(CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        return Ok(await _service.GetAsync(currentUser.EmpresaId, cancellationToken));
    }

    [HttpPatch("{id:guid}/lida")]
    public async Task<IActionResult> MarcarLida(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _service.MarcarComoLidaAsync(id, currentUser.EmpresaId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("lida-todas")]
    public async Task<IActionResult> MarcarTodasLidas(CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _service.MarcarTodasComoLidasAsync(currentUser.EmpresaId, cancellationToken);
        return NoContent();
    }
}
