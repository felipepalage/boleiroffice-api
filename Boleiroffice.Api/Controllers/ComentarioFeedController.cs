using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/feed/{desafioId:guid}/comentarios")]
public sealed class ComentarioFeedController : ControllerBase
{
    private readonly IComentarioFeedService _service;

    public ComentarioFeedController(IComentarioFeedService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ComentarioResponse>>> GetByDesafio(
        Guid desafioId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByDesafioAsync(desafioId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ComentarioResponse>> Comentar(
        Guid desafioId,
        [FromBody] ComentarRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.AddAsync(desafioId, request, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _service.DeleteAsync(id, currentUser, cancellationToken);
        return NoContent();
    }
}
