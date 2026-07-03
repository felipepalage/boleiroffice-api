using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IMensagemDesafioRepository
{
    Task<IReadOnlyList<MensagemDesafio>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default);
    Task AddAsync(MensagemDesafio mensagem, CancellationToken cancellationToken = default);
}
