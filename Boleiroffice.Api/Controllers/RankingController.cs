using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/ranking")]
public sealed class RankingController : ControllerBase
{
    private readonly IRankingService _rankingService;

    public RankingController(IRankingService rankingService)
    {
        _rankingService = rankingService;
    }

    private static DateOnly? PeriodToDataInicio(string? periodo) => periodo?.ToLowerInvariant() switch
    {
        "semanal" => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
        "mensal" => DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
        _ => null,
    };

    [HttpGet]
    public async Task<ActionResult<PagedResult<RankingResponse>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? periodo = null, CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters { Page = page, PageSize = pageSize, DataInicio = PeriodToDataInicio(periodo) };
        var response = await _rankingService.GetAsync(pagination, cancellationToken);
        return Ok(response);
    }

    [HttpGet("artilheiros")]
    public async Task<ActionResult<PagedResult<ArtilheiroRankingResponse>>> GetScorers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? periodo = null, CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters { Page = page, PageSize = pageSize, DataInicio = PeriodToDataInicio(periodo) };
        var response = await _rankingService.GetScorersAsync(pagination, cancellationToken);
        return Ok(response);
    }

    [HttpGet("reputacao")]
    public async Task<ActionResult<PagedResult<ReputacaoRankingResponse>>> GetReputation([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? periodo = null, CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters { Page = page, PageSize = pageSize, DataInicio = PeriodToDataInicio(periodo) };
        var response = await _rankingService.GetReputationAsync(pagination, cancellationToken);
        return Ok(response);
    }
}
