using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class FinanceiroRepository : IFinanceiroRepository
{
    private readonly ApplicationDbContext _context;

    public FinanceiroRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FinanceiroItem>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        return await _context.FinanceiroItens
            .Include(x => x.Time)
            .Where(x => x.TimeId == timeId)
            .OrderBy(x => x.Pago).ThenBy(x => x.DataVencimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<FinanceiroItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.FinanceiroItens.FindAsync([id], cancellationToken);

    public async Task AddAsync(FinanceiroItem item, CancellationToken cancellationToken = default)
    {
        item.Id = Guid.NewGuid();
        _context.FinanceiroItens.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FinanceiroItem item, CancellationToken cancellationToken = default)
    {
        _context.FinanceiroItens.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(FinanceiroItem item, CancellationToken cancellationToken = default)
    {
        _context.FinanceiroItens.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
