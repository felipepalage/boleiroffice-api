using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Amistoso;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/amistoso")]
public sealed class AmistosoController : ControllerBase
{
    private readonly IAmistosoService _service;

    public AmistosoController(IAmistosoService service) => _service = service;

    // ---------- Elenco (Geral / Pagamentos) ----------

    [HttpGet("jogadores")]
    public async Task<ActionResult<IReadOnlyList<JogadorAmistosoResponse>>> GetJogadores(CancellationToken cancellationToken)
        => Ok(await _service.GetJogadoresAsync(User.GetCurrentUser().EmpresaId, cancellationToken));

    [HttpPost("jogadores")]
    public async Task<ActionResult<JogadorAmistosoResponse>> AddJogador([FromBody] JogadorAmistosoCreateRequest request, CancellationToken cancellationToken)
        => Ok(await _service.AddJogadorAsync(User.GetCurrentUser().EmpresaId, request, cancellationToken));

    [HttpDelete("jogadores/{id:guid}")]
    public async Task<IActionResult> RemoverJogador(Guid id, CancellationToken cancellationToken)
    {
        await _service.RemoverJogadorAsync(User.GetCurrentUser().EmpresaId, id, cancellationToken);
        return NoContent();
    }

    [HttpPut("jogadores/{id:guid}/pagamento")]
    public async Task<ActionResult<JogadorAmistosoResponse>> AtualizarPagamento(Guid id, [FromBody] PagamentoUpdateRequest request, CancellationToken cancellationToken)
        => Ok(await _service.AtualizarPagamentoAsync(User.GetCurrentUser().EmpresaId, id, request, cancellationToken));

    [HttpPost("pagamentos/zerar")]
    public async Task<IActionResult> ZerarPagamentos(CancellationToken cancellationToken)
    {
        await _service.ZerarPagamentosAsync(User.GetCurrentUser().EmpresaId, cancellationToken);
        return NoContent();
    }

    // ---------- Sorteio / Times ----------

    [HttpPost("sortear")]
    public async Task<ActionResult<IReadOnlyList<TimeAmistosoResponse>>> Sortear([FromBody] SortearRequest request, CancellationToken cancellationToken)
        => Ok(await _service.SortearAsync(User.GetCurrentUser().EmpresaId, request, cancellationToken));

    [HttpGet("times")]
    public async Task<ActionResult<IReadOnlyList<TimeAmistosoResponse>>> GetTimes(CancellationToken cancellationToken)
        => Ok(await _service.GetTimesAsync(User.GetCurrentUser().EmpresaId, cancellationToken));

    // ---------- Partidas (Rachão) ----------

    [HttpPost("partidas")]
    public async Task<ActionResult<PartidaAmistosoResponse>> IniciarPartida([FromBody] IniciarPartidaRequest request, CancellationToken cancellationToken)
        => Ok(await _service.IniciarPartidaAsync(User.GetCurrentUser().EmpresaId, request, cancellationToken));

    [HttpPost("partidas/{id:guid}/gols")]
    public async Task<ActionResult<PartidaAmistosoResponse>> RegistrarGol(Guid id, [FromBody] RegistrarGolRequest request, CancellationToken cancellationToken)
        => Ok(await _service.RegistrarGolAsync(User.GetCurrentUser().EmpresaId, id, request, cancellationToken));

    [HttpDelete("partidas/{id:guid}/gols/{golId:guid}")]
    public async Task<ActionResult<PartidaAmistosoResponse>> AnularGol(Guid id, Guid golId, CancellationToken cancellationToken)
        => Ok(await _service.AnularGolAsync(User.GetCurrentUser().EmpresaId, id, golId, cancellationToken));

    [HttpPost("partidas/{id:guid}/finalizar")]
    public async Task<ActionResult<PartidaAmistosoResponse>> FinalizarPartida(Guid id, [FromBody] FinalizarPartidaRequest request, CancellationToken cancellationToken)
        => Ok(await _service.FinalizarPartidaAsync(User.GetCurrentUser().EmpresaId, id, request, cancellationToken));

    [HttpGet("partidas/{id:guid}")]
    public async Task<ActionResult<PartidaAmistosoResponse>> GetPartida(Guid id, CancellationToken cancellationToken)
        => Ok(await _service.GetPartidaAsync(User.GetCurrentUser().EmpresaId, id, cancellationToken));

    [HttpGet("partidas")]
    public async Task<ActionResult<PagedResult<PartidaAmistosoResponse>>> GetPartidas(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => Ok(await _service.GetPartidasAsync(User.GetCurrentUser().EmpresaId, new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken));

    // ---------- Ranking / resumo do dia ----------

    [HttpGet("ranking/artilheiros")]
    public async Task<ActionResult<IReadOnlyList<ArtilheiroAmistosoResponse>>> GetArtilheiros(CancellationToken cancellationToken)
        => Ok(await _service.GetArtilheirosAsync(User.GetCurrentUser().EmpresaId, cancellationToken));

    [HttpGet("ranking/garcons")]
    public async Task<ActionResult<IReadOnlyList<ArtilheiroAmistosoResponse>>> GetGarcons(CancellationToken cancellationToken)
        => Ok(await _service.GetGarconsAsync(User.GetCurrentUser().EmpresaId, cancellationToken));

    [HttpGet("resumo-dia")]
    public async Task<ActionResult<ResumoDiaResponse>> GetResumoDia([FromQuery] DateOnly? data, CancellationToken cancellationToken)
        => Ok(await _service.GetResumoDiaAsync(User.GetCurrentUser().EmpresaId, data, cancellationToken));
}
