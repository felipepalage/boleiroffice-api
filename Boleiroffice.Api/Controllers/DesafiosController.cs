using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/desafios")]
public sealed class DesafiosController : ControllerBase
{
    private readonly IDesafioService _desafioService;

    public DesafiosController(IDesafioService desafioService)
    {
        _desafioService = desafioService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DesafioResponse>> Create([FromBody] DesafioCreateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.CreateAsync(request, currentUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("abertos")]
    public async Task<ActionResult<PagedResult<DesafioResponse>>> GetOpen(
        [FromQuery] string? bairro,
        [FromQuery] DateOnly? data,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response = await _desafioService.GetOpenAsync(bairro, data, new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/aceitar")]
    public async Task<ActionResult<DesafioResponse>> Accept(Guid id, [FromBody] AcceptDesafioRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.AcceptAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<DesafioResponse>> Cancel(Guid id, [FromBody] CancelDesafioRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.CancelAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/resultado")]
    public async Task<ActionResult<DesafioResponse>> RegisterResult(Guid id, [FromBody] RegisterResultRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.RegisterResultAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/resultado/confirmar")]
    public async Task<ActionResult<DesafioResponse>> ConfirmResult(Guid id, [FromBody] ConfirmResultRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.ConfirmResultAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/artilheiros")]
    public async Task<ActionResult<DesafioResponse>> RegisterScorers(Guid id, [FromBody] RegistrarArtilheirosRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _desafioService.RegisterScorersAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DesafioResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _desafioService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("time/{timeId:guid}")]
    public async Task<ActionResult<PagedResult<DesafioResponse>>> GetByTime(Guid timeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await _desafioService.GetByTimeIdAsync(timeId, new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("sugeridos")]
    public async Task<ActionResult<PagedResult<SuggestedChallengeResponse>>> GetSuggested([FromQuery] Guid timeId, [FromQuery] DateOnly dataJogo, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await _desafioService.GetSuggestedAsync(timeId, dataJogo, new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }
}
