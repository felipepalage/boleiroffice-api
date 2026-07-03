using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/feed/{desafioId:guid}/reacoes")]
public sealed class ReacaoFeedController : ControllerBase
{
    private readonly IReacaoFeedService _reacaoService;

    public ReacaoFeedController(IReacaoFeedService reacaoService)
    {
        _reacaoService = reacaoService;
    }

    [HttpGet]
    public async Task<ActionResult<ReacoesDesafioResponse>> Get(Guid desafioId, CancellationToken cancellationToken)
    {
        Guid? empresaId = User.Identity?.IsAuthenticated == true
            ? User.GetCurrentUser().EmpresaId
            : null;

        var response = await _reacaoService.GetByDesafioAsync(desafioId, empresaId, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReacoesDesafioResponse>> Reagir(
        Guid desafioId,
        [FromBody] ReagirFeedRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _reacaoService.ReagirAsync(desafioId, request, currentUser, cancellationToken);
        return Ok(response);
    }
}
