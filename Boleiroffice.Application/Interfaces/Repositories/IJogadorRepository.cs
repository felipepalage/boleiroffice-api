using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IJogadorRepository
{
    Task AddAsync(Jogador jogador, CancellationToken cancellationToken);
    Task DeleteAsync(Jogador jogador, CancellationToken cancellationToken);
    Task<Jogador?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Jogador>> GetByTimeIdAsync(Guid timeId, CancellationToken cancellationToken);
    Task<bool> JerseyNumberExistsAsync(Guid timeId, int numeroCamisa, CancellationToken cancellationToken);
    Task<(int TotalGols, int JogosComGol, int TotalJogosTime)> GetGoalStatsAsync(Guid timeId, string nomeAutor, CancellationToken cancellationToken);
}
