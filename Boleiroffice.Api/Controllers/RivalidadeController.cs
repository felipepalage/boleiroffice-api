using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/times/{timeId:guid}/rivals")]
public sealed class RivalidadeController : ControllerBase
{
    private readonly IRivalidadeService _service;

    public RivalidadeController(IRivalidadeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<RividadesResponse>> GetRivals(
        Guid timeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRivaisAsync(timeId, cancellationToken);
        return Ok(result);
    }
}
