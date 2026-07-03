using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IReacaoFeedRepository
{
    Task<IReadOnlyList<ReacaoFeed>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default);
    Task<ReacaoFeed?> GetByDesafioAndEmpresaAsync(Guid desafioId, Guid empresaId, CancellationToken cancellationToken = default);
    Task AddAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default);
    Task DeleteAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default);
}
