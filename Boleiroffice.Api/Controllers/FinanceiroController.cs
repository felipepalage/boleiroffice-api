using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Financeiro;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/financeiro")]
[Authorize]
public sealed class FinanceiroController : ControllerBase
{
    private readonly IFinanceiroService _service;

    public FinanceiroController(IFinanceiroService service)
    {
        _service = service;
    }

    [HttpGet("{timeId:guid}")]
    public async Task<ActionResult<FinanceiroSummaryResponse>> GetByTime(
        Guid timeId, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.GetByTimeAsync(timeId, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{timeId:guid}")]
    public async Task<ActionResult<FinanceiroItemResponse>> Create(
        Guid timeId,
        [FromBody] FinanceiroItemRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.CreateAsync(timeId, request, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/pagar")]
    public async Task<ActionResult<FinanceiroItemResponse>> MarcarPago(
        Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.MarcarPagoAsync(id, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _service.DeleteAsync(id, currentUser, cancellationToken);
        return NoContent();
    }
}
