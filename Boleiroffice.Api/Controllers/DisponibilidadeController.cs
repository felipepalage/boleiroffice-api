using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Disponibilidade;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/disponibilidades")]
public sealed class DisponibilidadeController : ControllerBase
{
    private readonly IDisponibilidadeService _service;

    public DisponibilidadeController(IDisponibilidadeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BrowseDisponibilidadeItem>>> Browse(
        [FromQuery] string? cidade,
        [FromQuery] string? bairro,
        [FromQuery] int? diaSemana,
        [FromQuery] Guid? meuTimeId,
        CancellationToken cancellationToken)
    {
        var result = await _service.BrowseAsync(cidade, bairro, diaSemana, meuTimeId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("meu-time/{timeId:guid}")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<DisponibilidadeResponse>>> GetByTime(
        Guid timeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByTimeAsync(timeId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<DisponibilidadeResponse>> Create(
        [FromBody] CreateDisponibilidadeRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.CreateAsync(request, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<DisponibilidadeResponse>> Update(
        Guid id,
        [FromBody] DisponibilidadeRequest request,
        [FromQuery] Guid timeId,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, timeId, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        Guid id, [FromQuery] Guid timeId, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, timeId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/desafiar")]
    [Authorize]
    public async Task<IActionResult> Desafiar(
        Guid id,
        [FromBody] DesafiarDeSlotBodyRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _service.DesafiarDeSlotAsync(id, request, currentUser, cancellationToken);
        return Ok(new { message = "Desafio enviado com sucesso!" });
    }
}
