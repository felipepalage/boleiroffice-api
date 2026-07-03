using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class TemporadaRepository : ITemporadaRepository
{
    private readonly ApplicationDbContext _context;

    public TemporadaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Temporada>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Temporadas
            .OrderByDescending(x => x.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<Temporada?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Temporadas.FindAsync([id], cancellationToken);

    public async Task<Temporada?> GetAtivaAsync(CancellationToken cancellationToken = default)
        => await _context.Temporadas.FirstOrDefaultAsync(x => x.Ativa, cancellationToken);

    public async Task<bool> ExistsByNomeAsync(string nome, CancellationToken cancellationToken = default)
        => await _context.Temporadas.AnyAsync(x => x.Nome == nome, cancellationToken);

    public async Task AddAsync(Temporada temporada, CancellationToken cancellationToken = default)
    {
        temporada.Id = Guid.NewGuid();
        _context.Temporadas.Add(temporada);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Temporada temporada, CancellationToken cancellationToken = default)
    {
        _context.Temporadas.Update(temporada);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Temporada temporada, CancellationToken cancellationToken = default)
    {
        _context.Temporadas.Remove(temporada);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
