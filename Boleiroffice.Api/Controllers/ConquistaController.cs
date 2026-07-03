using Boleiroffice.Application.DTOs.Conquista;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/times/{timeId:guid}/conquistas")]
public sealed class ConquistaController : ControllerBase
{
    private readonly IConquistaService _service;

    public ConquistaController(IConquistaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ConquistasTimeResponse>> GetConquistas(
        Guid timeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetConquistasAsync(timeId, cancellationToken);
        return Ok(result);
    }
}
