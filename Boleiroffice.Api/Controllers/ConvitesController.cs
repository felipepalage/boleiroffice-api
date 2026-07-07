using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.DTOs.Convites;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/convites")]
public sealed class ConvitesController : ControllerBase
{
    private readonly IConviteMembroService _conviteService;

    public ConvitesController(IConviteMembroService conviteService)
    {
        _conviteService = conviteService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ConviteResponse>> Criar(CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _conviteService.CriarAsync(currentUser.EmpresaId, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{token}")]
    public async Task<ActionResult<ConviteInfoResponse>> GetInfo(string token, CancellationToken cancellationToken)
    {
        var info = await _conviteService.GetInfoAsync(token, cancellationToken);
        return info is null ? NotFound() : Ok(info);
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("{token}/aceitar")]
    public async Task<ActionResult<AuthResponse>> Aceitar(string token, [FromBody] AceitarConviteRequest request, CancellationToken cancellationToken)
    {
        var response = await _conviteService.AceitarAsync(token, request, cancellationToken);
        return Ok(response);
    }
}
