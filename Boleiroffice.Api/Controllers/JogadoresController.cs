using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Jogadores;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/jogadores")]
public sealed class JogadoresController : ControllerBase
{
    private readonly IJogadorService _jogadorService;

    public JogadoresController(IJogadorService jogadorService)
    {
        _jogadorService = jogadorService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<JogadorResponse>> Create([FromBody] JogadorCreateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _jogadorService.CreateAsync(request, currentUser, cancellationToken);
        return CreatedAtAction(nameof(GetByTime), new { timeId = response.TimeId }, response);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _jogadorService.DeleteAsync(id, currentUser, cancellationToken);
        return NoContent();
    }

    [HttpGet("time/{timeId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<JogadorResponse>>> GetByTime(Guid timeId, CancellationToken cancellationToken)
    {
        var response = await _jogadorService.GetByTimeIdAsync(timeId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}/perfil")]
    public async Task<ActionResult<JogadorPerfilResponse>> GetPerfil(Guid id, CancellationToken cancellationToken)
    {
        var response = await _jogadorService.GetPerfilAsync(id, cancellationToken);
        return Ok(response);
    }
}
