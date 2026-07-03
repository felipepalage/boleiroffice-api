using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Amistoso;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IAmistosoService
{
    // Elenco (Geral / Pagamentos)
    Task<IReadOnlyList<JogadorAmistosoResponse>> GetJogadoresAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<JogadorAmistosoResponse> AddJogadorAsync(Guid empresaId, JogadorAmistosoCreateRequest request, CancellationToken cancellationToken);
    Task RemoverJogadorAsync(Guid empresaId, Guid jogadorId, CancellationToken cancellationToken);
    Task<JogadorAmistosoResponse> AtualizarPagamentoAsync(Guid empresaId, Guid jogadorId, PagamentoUpdateRequest request, CancellationToken cancellationToken);
    Task ZerarPagamentosAsync(Guid empresaId, CancellationToken cancellationToken);

    // Sorteio / Times
    Task<IReadOnlyList<TimeAmistosoResponse>> SortearAsync(Guid empresaId, SortearRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<TimeAmistosoResponse>> GetTimesAsync(Guid empresaId, CancellationToken cancellationToken);

    // Partidas (Rachão)
    Task<PartidaAmistosoResponse> IniciarPartidaAsync(Guid empresaId, IniciarPartidaRequest request, CancellationToken cancellationToken);
    Task<PartidaAmistosoResponse> RegistrarGolAsync(Guid empresaId, Guid partidaId, RegistrarGolRequest request, CancellationToken cancellationToken);
    Task<PartidaAmistosoResponse> FinalizarPartidaAsync(Guid empresaId, Guid partidaId, FinalizarPartidaRequest request, CancellationToken cancellationToken);
    Task<PartidaAmistosoResponse> GetPartidaAsync(Guid empresaId, Guid partidaId, CancellationToken cancellationToken);
    Task<PagedResult<PartidaAmistosoResponse>> GetPartidasAsync(Guid empresaId, PaginationParameters pagination, CancellationToken cancellationToken);

    // Ranking / resumo do dia
    Task<IReadOnlyList<ArtilheiroAmistosoResponse>> GetArtilheirosAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ArtilheiroAmistosoResponse>> GetGarconsAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<ResumoDiaResponse> GetResumoDiaAsync(Guid empresaId, DateOnly? data, CancellationToken cancellationToken);
}
