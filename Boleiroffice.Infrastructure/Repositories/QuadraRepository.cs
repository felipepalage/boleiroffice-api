using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class QuadraRepository : IQuadraRepository
{
    private readonly ApplicationDbContext _context;

    public QuadraRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Quadra quadra, CancellationToken cancellationToken)
    {
        await _context.Quadras.AddAsync(quadra, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Quadra quadra, CancellationToken cancellationToken)
    {
        _context.Quadras.Update(quadra);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Quadra?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _context.Quadras
            .AsNoTracking()
            .Include(q => q.Avaliacoes).ThenInclude(a => a.Empresa)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

    public async Task<PagedResult<Quadra>> GetPagedAsync(string? bairro, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Quadras
            .AsNoTracking()
            .Include(q => q.Avaliacoes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(bairro))
        {
            var b = bairro.Trim().ToLower();
            query = query.Where(q => q.Bairro.ToLower().Contains(b) || q.Cidade.ToLower().Contains(b));
        }

        query = query.OrderBy(q => q.Nome);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Quadra>.Create(items, pagination.Page, pagination.PageSize, total);
    }

    public async Task AddAvaliacaoAsync(AvaliacaoQuadra avaliacao, CancellationToken cancellationToken)
    {
        await _context.AvaliacoesQuadra.AddAsync(avaliacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> JaAvaliouAsync(Guid quadraId, Guid empresaId, CancellationToken cancellationToken)
        => _context.AvaliacoesQuadra.AnyAsync(a => a.QuadraId == quadraId && a.EmpresaId == empresaId, cancellationToken);
}
