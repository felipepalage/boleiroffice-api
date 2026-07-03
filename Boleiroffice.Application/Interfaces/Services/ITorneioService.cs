using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Torneios;

namespace Boleiroffice.Application.Interfaces.Services;

public interface ITorneioService
{
    Task<TorneioResponse> CreateAsync(TorneioCreateRequest request, Guid empresaId, CancellationToken cancellationToken);
    Task<TorneioResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<TorneioResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task<TorneioResponse> InscreverTimeAsync(Guid torneioId, TorneioInscricaoRequest request, Guid empresaId, CancellationToken cancellationToken);
    Task<TorneioResponse> IniciarAsync(Guid id, Guid empresaId, CancellationToken cancellationToken);
    Task<PartidaTorneioResponse> AgendarPartidaAsync(Guid torneioId, AgendarPartidaRequest request, Guid empresaId, CancellationToken cancellationToken);
    Task<PartidaTorneioResponse> RegistrarResultadoAsync(Guid torneioId, Guid partidaId, RegistrarResultadoPartidaRequest request, Guid empresaId, CancellationToken cancellationToken);
}
