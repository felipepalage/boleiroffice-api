using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Presenca;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/desafios/{desafioId:guid}/presenca")]
public sealed class PresencaController : ControllerBase
{
    private readonly IPresencaService _presencaService;

    public PresencaController(IPresencaService presencaService)
    {
        _presencaService = presencaService;
    }

    [HttpGet]
    public async Task<ActionResult<PresencaDesafioResponse>> Get(Guid desafioId, CancellationToken cancellationToken)
    {
        var response = await _presencaService.GetByDesafioAsync(desafioId, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PresencaResponse>> Confirmar(
        Guid desafioId,
        [FromBody] ConfirmarPresencaRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _presencaService.ConfirmarAsync(desafioId, request, currentUser, cancellationToken);
        return Ok(response);
    }
}
