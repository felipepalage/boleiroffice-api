using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Mural;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/empresas/{empresaId:guid}/mural")]
public sealed class MuralController : ControllerBase
{
    private readonly IPostMuralService _service;

    public MuralController(IPostMuralService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PostMuralResponse>>> GetMural(
        Guid empresaId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByEmpresaAsync(empresaId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostMuralResponse>> Create(
        Guid empresaId,
        [FromBody] PostMuralRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.CreateAsync(empresaId, request, currentUser, cancellationToken);
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
