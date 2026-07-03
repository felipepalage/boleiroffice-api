using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface INotificacaoRepository
{
    Task AddAsync(Notificacao notificacao, CancellationToken cancellationToken);
    Task<IReadOnlyList<Notificacao>> GetByEmpresaAsync(Guid empresaId, int limit, CancellationToken cancellationToken);
    Task MarcarComoLidaAsync(Guid id, Guid empresaId, CancellationToken cancellationToken);
    Task MarcarTodasComoLidasAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<int> GetUnreadCountAsync(Guid empresaId, CancellationToken cancellationToken);
}
