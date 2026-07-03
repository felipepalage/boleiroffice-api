using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class TimeRepository : ITimeRepository
{
    private readonly ApplicationDbContext _context;

    public TimeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Time time, CancellationToken cancellationToken)
    {
        await _context.Times.AddAsync(time, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Time time, CancellationToken cancellationToken)
    {
        _context.Times.Update(time);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameWithinCompanyAsync(Guid empresaId, string nome, CancellationToken cancellationToken)
    {
        var normalizedName = nome.Trim().ToLower();
        return _context.Times.AnyAsync(x => x.EmpresaId == empresaId && x.Nome.ToLower() == normalizedName, cancellationToken);
    }

    public Task<Time?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Times
            .Include(x => x.Empresa)
            .Include(x => x.Jogadores)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Time?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Times
            .AsNoTracking()
            .Include(x => x.Empresa)
            .Include(x => x.Jogadores.OrderBy(j => j.NumeroCamisa))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Time>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Times
            .AsNoTracking()
            .Include(x => x.Empresa)
            .Include(x => x.Jogadores)
            .OrderBy(x => x.Nome);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Time>.Create(items, pagination.Page, pagination.PageSize, totalCount);
    }

    public async Task DeleteAsync(Time time, CancellationToken cancellationToken)
    {
        _context.Times.Remove(time);
        await _context.SaveChangesAsync(cancellationToken);
    }
}