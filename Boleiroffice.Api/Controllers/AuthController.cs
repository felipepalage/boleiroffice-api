using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICnpjLookupService _cnpjLookupService;

    public AuthController(IAuthService authService, ICnpjLookupService cnpjLookupService)
    {
        _authService = authService;
        _cnpjLookupService = cnpjLookupService;
    }

    [HttpGet("cnpj/{cnpj}")]
    public async Task<ActionResult<CnpjInfoResponse>> ConsultarCnpj(string cnpj, CancellationToken cancellationToken)
    {
        var info = await _cnpjLookupService.ConsultarAsync(cnpj, cancellationToken);
        return info is null ? NotFound() : Ok(info);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }
}
