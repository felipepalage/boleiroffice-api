using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IRachaoEventoRepository
{
    Task AddAsync(RachaoEvento evento, CancellationToken cancellationToken);
    Task AddConfirmacaoAsync(RachaoConfirmacao confirmacao, CancellationToken cancellationToken);
    Task RemoveConfirmacaoAsync(RachaoConfirmacao confirmacao, CancellationToken cancellationToken);
    Task<RachaoEvento?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task<RachaoEvento?> GetAtivoByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RachaoEvento>> GetPendentesSorteioAsync(DateTime agora, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
    Task<int> RemoverAntigosAsync(DateTime cutoff, CancellationToken cancellationToken);
}