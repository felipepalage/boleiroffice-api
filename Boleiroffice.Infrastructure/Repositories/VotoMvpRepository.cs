using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class VotoMvpRepository : IVotoMvpRepository
{
    private readonly ApplicationDbContext _context;

    public VotoMvpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VotoMvp?> GetByDesafioAndEmpresaAsync(Guid desafioId, Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await _context.VotosMvp
            .FirstOrDefaultAsync(v => v.DesafioId == desafioId && v.VotantePorEmpresaId == empresaId, cancellationToken);
    }

    public async Task<IReadOnlyList<VotoMvp>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default)
    {
        return await _context.VotosMvp
            .Include(v => v.JogadorVotado)
            .ThenInclude(j => j!.Time)
            .Where(v => v.DesafioId == desafioId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(VotoMvp voto, CancellationToken cancellationToken = default)
    {
        if (voto.Id == Guid.Empty)
        {
            voto.Id = Guid.NewGuid();
            _context.VotosMvp.Add(voto);
        }
        else
        {
            _context.VotosMvp.Update(voto);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
