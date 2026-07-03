using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class NotificacaoRepository : INotificacaoRepository
{
    private readonly ApplicationDbContext _context;

    public NotificacaoRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Notificacao notificacao, CancellationToken cancellationToken)
    {
        await _context.Notificacoes.AddAsync(notificacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Notificacao>> GetByEmpresaAsync(Guid empresaId, int limit, CancellationToken cancellationToken)
        => _context.Notificacoes
            .AsNoTracking()
            .Where(n => n.EmpresaId == empresaId)
            .OrderByDescending(n => n.DataCriacao)
            .Take(limit)
            .ToArrayAsync(cancellationToken)
            .ContinueWith<IReadOnlyList<Notificacao>>(t => t.Result, cancellationToken);

    public async Task MarcarComoLidaAsync(Guid id, Guid empresaId, CancellationToken cancellationToken)
    {
        var n = await _context.Notificacoes.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == empresaId, cancellationToken);
        if (n is null) return;
        n.Lida = true;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarcarTodasComoLidasAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        await _context.Notificacoes
            .Where(n => n.EmpresaId == empresaId && !n.Lida)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lida, true), cancellationToken);
    }

    public Task<int> GetUnreadCountAsync(Guid empresaId, CancellationToken cancellationToken)
        => _context.Notificacoes.CountAsync(n => n.EmpresaId == empresaId && !n.Lida, cancellationToken);
}
