using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class DisponibilidadeTimeRepository : IDisponibilidadeTimeRepository
{
    private readonly ApplicationDbContext _context;

    public DisponibilidadeTimeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DisponibilidadeTime>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        return await _context.DisponibilidadesTime
            .Include(x => x.Time)
            .Where(x => x.TimeId == timeId)
            .OrderBy(x => x.DiaSemana).ThenBy(x => x.Horario)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DisponibilidadeTime>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DisponibilidadesTime
            .Include(x => x.Time)
            .Where(x => x.Ativo)
            .OrderBy(x => x.Cidade).ThenBy(x => x.Bairro)
            .ToListAsync(cancellationToken);
    }

    public async Task<DisponibilidadeTime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DisponibilidadesTime
            .Include(x => x.Time)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsConflictAsync(Guid timeId, int diaSemana, TimeOnly horario, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        var query = _context.DisponibilidadesTime
            .Where(x => x.TimeId == timeId && x.DiaSemana == diaSemana && x.Horario == horario);

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default)
    {
        item.Id = Guid.NewGuid();
        _context.DisponibilidadesTime.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default)
    {
        _context.DisponibilidadesTime.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DisponibilidadeTime item, CancellationToken cancellationToken = default)
    {
        _context.DisponibilidadesTime.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
