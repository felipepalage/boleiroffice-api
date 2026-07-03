using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Quadras;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/quadras")]
public sealed class QuadrasController : ControllerBase
{
    private readonly IQuadraService _quadraService;

    public QuadrasController(IQuadraService quadraService) => _quadraService = quadraService;

    [HttpGet]
    public async Task<ActionResult<PagedResult<QuadraResponse>>> GetAll(
        [FromQuery] string? bairro,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _quadraService.GetPagedAsync(bairro, new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuadraResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _quadraService.GetByIdAsync(id, cancellationToken));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<QuadraResponse>> Create([FromBody] QuadraCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await _quadraService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPost("{id:guid}/avaliacoes")]
    public async Task<ActionResult<QuadraResponse>> Avaliar(Guid id, [FromBody] AvaliacaoCreateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _quadraService.AvaliarAsync(id, request, currentUser.EmpresaId, cancellationToken);
        return Ok(result);
    }
}
