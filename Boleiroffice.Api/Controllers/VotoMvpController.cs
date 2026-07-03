using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Mvp;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/desafios/{desafioId:guid}/mvp")]
public sealed class VotoMvpController : ControllerBase
{
    private readonly IVotoMvpService _votoMvpService;

    public VotoMvpController(IVotoMvpService votoMvpService)
    {
        _votoMvpService = votoMvpService;
    }

    [HttpGet]
    public async Task<ActionResult<VotacaoMvpResponse>> Get(Guid desafioId, CancellationToken cancellationToken)
    {
        Guid? empresaId = User.Identity?.IsAuthenticated == true
            ? User.GetCurrentUser().EmpresaId
            : null;

        var response = await _votoMvpService.GetByDesafioAsync(desafioId, empresaId, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<VotacaoMvpResponse>> Votar(
        Guid desafioId,
        [FromBody] VotarMvpRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _votoMvpService.VotarAsync(desafioId, request, currentUser, cancellationToken);
        return Ok(response);
    }
}
