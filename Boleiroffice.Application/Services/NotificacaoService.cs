using Boleiroffice.Application.DTOs.Notificacoes;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Application.Services;

public sealed class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _repo;

    public NotificacaoService(INotificacaoRepository repo)
    {
        _repo = repo;
    }

    public async Task<NotificacoesResumoResponse> GetAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        var itens = await _repo.GetByEmpresaAsync(empresaId, 50, cancellationToken);
        var naoLidas = itens.Count(x => !x.Lida);
        var response = itens.Select(n => new NotificacaoResponse(n.Id, n.Tipo, n.Titulo, n.Mensagem, n.Url, n.Lida, n.DataCriacao)).ToList();
        return new NotificacoesResumoResponse(naoLidas, response);
    }

    public Task MarcarComoLidaAsync(Guid id, Guid empresaId, CancellationToken cancellationToken)
        => _repo.MarcarComoLidaAsync(id, empresaId, cancellationToken);

    public Task MarcarTodasComoLidasAsync(Guid empresaId, CancellationToken cancellationToken)
        => _repo.MarcarTodasComoLidasAsync(empresaId, cancellationToken);
}
