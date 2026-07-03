using Boleiroffice.Application.Common.Models;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface ITorneioRepository
{
    Task AddAsync(Torneio torneio, CancellationToken cancellationToken);
    Task UpdateAsync(Torneio torneio, CancellationToken cancellationToken);
    Task<Torneio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Torneio>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
    Task AddInscricaoAsync(TorneioInscricao inscricao, CancellationToken cancellationToken);
    Task AddPartidaAsync(PartidaTorneio partida, CancellationToken cancellationToken);
    Task<PartidaTorneio?> GetPartidaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdatePartidaAsync(PartidaTorneio partida, CancellationToken cancellationToken);
}
