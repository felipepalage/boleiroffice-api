using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class EmpresaRepository : IEmpresaRepository
{
    private readonly ApplicationDbContext _context;

    public EmpresaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Empresa empresa, CancellationToken cancellationToken)
    {
        await _context.Empresas.AddAsync(empresa, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Empresa empresa, CancellationToken cancellationToken)
    {
        _context.Empresas.Update(empresa);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string nome, CancellationToken cancellationToken)
    {
        var normalizedName = nome.Trim().ToLower();
        return _context.Empresas.AnyAsync(x => x.Nome.ToLower() == normalizedName, cancellationToken);
    }

    public Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        return _context.Empresas.AnyAsync(x => x.Cnpj == cnpj, cancellationToken);
    }

    public Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        return _context.Empresas.FirstOrDefaultAsync(x => x.Cnpj == cnpj, cancellationToken);
    }

    public Task<Empresa?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Empresas
            .Include(x => x.Times)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Empresa>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Empresas
            .AsNoTracking()
            .Include(x => x.Times)
            .OrderBy(x => x.Nome);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Empresa>.Create(items, pagination.Page, pagination.PageSize, totalCount);
    }
}