using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class ReacaoFeedRepository : IReacaoFeedRepository
{
    private readonly ApplicationDbContext _context;

    public ReacaoFeedRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ReacaoFeed>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default)
    {
        return await _context.ReacoesFeed
            .Where(r => r.DesafioId == desafioId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReacaoFeed?> GetByDesafioAndEmpresaAsync(Guid desafioId, Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await _context.ReacoesFeed
            .FirstOrDefaultAsync(r => r.DesafioId == desafioId && r.EmpresaId == empresaId, cancellationToken);
    }

    public async Task AddAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default)
    {
        _context.ReacoesFeed.Add(reacao);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default)
    {
        _context.ReacoesFeed.Update(reacao);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ReacaoFeed reacao, CancellationToken cancellationToken = default)
    {
        _context.ReacoesFeed.Remove(reacao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
