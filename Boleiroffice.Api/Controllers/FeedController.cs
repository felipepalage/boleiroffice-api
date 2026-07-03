using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/feed")]
public sealed class FeedController : ControllerBase
{
    private readonly IFeedService _feedService;

    public FeedController(IFeedService feedService)
    {
        _feedService = feedService;
    }

    [HttpGet("jogos")]
    public async Task<ActionResult<PagedResult<FeedJogoResponse>>> GetJogos([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await _feedService.GetJogosAsync(new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }
}
