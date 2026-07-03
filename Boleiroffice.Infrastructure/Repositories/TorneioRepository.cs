using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class TorneioRepository : ITorneioRepository
{
    private readonly ApplicationDbContext _context;

    public TorneioRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Torneio torneio, CancellationToken cancellationToken)
    {
        await _context.Torneios.AddAsync(torneio, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Torneio torneio, CancellationToken cancellationToken)
    {
        _context.Torneios.Update(torneio);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Torneio?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _context.Torneios
            .AsNoTracking()
            .Include(t => t.EmpresaOrganizadora)
            .Include(t => t.Inscricoes)
                .ThenInclude(i => i.Time).ThenInclude(t => t.Empresa)
            .Include(t => t.Partidas)
                .ThenInclude(p => p.TimeMandante)
            .Include(t => t.Partidas)
                .ThenInclude(p => p.TimeVisitante)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<PagedResult<Torneio>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Torneios
            .AsNoTracking()
            .Include(t => t.EmpresaOrganizadora)
            .Include(t => t.Inscricoes)
            .OrderByDescending(t => t.DataCriacao);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<Torneio>.Create(items, pagination.Page, pagination.PageSize, total);
    }

    public async Task AddInscricaoAsync(TorneioInscricao inscricao, CancellationToken cancellationToken)
    {
        await _context.TorneioInscricoes.AddAsync(inscricao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPartidaAsync(PartidaTorneio partida, CancellationToken cancellationToken)
    {
        await _context.PartidasTorneio.AddAsync(partida, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<PartidaTorneio?> GetPartidaByIdAsync(Guid id, CancellationToken cancellationToken)
        => _context.PartidasTorneio
            .AsNoTracking()
            .Include(p => p.TimeMandante)
            .Include(p => p.TimeVisitante)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task UpdatePartidaAsync(PartidaTorneio partida, CancellationToken cancellationToken)
    {
        _context.PartidasTorneio.Update(partida);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
