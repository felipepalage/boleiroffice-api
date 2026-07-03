using Boleiroffice.Application.DTOs.Notificacoes;

namespace Boleiroffice.Application.Interfaces.Services;

public interface INotificacaoService
{
    Task<NotificacoesResumoResponse> GetAsync(Guid empresaId, CancellationToken cancellationToken);
    Task MarcarComoLidaAsync(Guid id, Guid empresaId, CancellationToken cancellationToken);
    Task MarcarTodasComoLidasAsync(Guid empresaId, CancellationToken cancellationToken);
}
