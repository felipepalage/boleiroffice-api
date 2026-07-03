using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IConfirmacaoPresencaRepository
{
    Task<ConfirmacaoPresenca?> GetAsync(Guid jogadorId, Guid desafioId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ConfirmacaoPresenca>> GetByDesafioIdAsync(Guid desafioId, CancellationToken cancellationToken);
    Task UpsertAsync(ConfirmacaoPresenca confirmacao, CancellationToken cancellationToken);
}
