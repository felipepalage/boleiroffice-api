using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Torneios;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/torneios")]
public sealed class TorneiosController : ControllerBase
{
    private readonly ITorneioService _service;

    public TorneiosController(ITorneioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResult<TorneioResponse>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => Ok(await _service.GetPagedAsync(new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TorneioResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _service.GetByIdAsync(id, cancellationToken));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TorneioResponse>> Create([FromBody] TorneioCreateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.CreateAsync(request, currentUser.EmpresaId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPost("{id:guid}/inscricao")]
    public async Task<ActionResult<TorneioResponse>> Inscrever(Guid id, [FromBody] TorneioInscricaoRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        return Ok(await _service.InscreverTimeAsync(id, request, currentUser.EmpresaId, cancellationToken));
    }

    [Authorize]
    [HttpPost("{id:guid}/iniciar")]
    public async Task<ActionResult<TorneioResponse>> Iniciar(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        return Ok(await _service.IniciarAsync(id, currentUser.EmpresaId, cancellationToken));
    }

    [Authorize]
    [HttpPost("{id:guid}/partidas")]
    public async Task<ActionResult<PartidaTorneioResponse>> AgendarPartida(Guid id, [FromBody] AgendarPartidaRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        return Ok(await _service.AgendarPartidaAsync(id, request, currentUser.EmpresaId, cancellationToken));
    }

    [Authorize]
    [HttpPatch("{id:guid}/partidas/{partidaId:guid}/resultado")]
    public async Task<ActionResult<PartidaTorneioResponse>> RegistrarResultado(Guid id, Guid partidaId, [FromBody] RegistrarResultadoPartidaRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        return Ok(await _service.RegistrarResultadoAsync(id, partidaId, request, currentUser.EmpresaId, cancellationToken));
    }
}
