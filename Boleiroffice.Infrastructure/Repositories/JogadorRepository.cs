using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class JogadorRepository : IJogadorRepository
{
    private readonly ApplicationDbContext _context;

    public JogadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Jogador jogador, CancellationToken cancellationToken)
    {
        await _context.Jogadores.AddAsync(jogador, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Jogador jogador, CancellationToken cancellationToken)
    {
        _context.Jogadores.Remove(jogador);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Jogador?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Jogadores
            .Include(x => x.Time)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Jogador>> GetByTimeIdAsync(Guid timeId, CancellationToken cancellationToken)
    {
        return await _context.Jogadores
            .AsNoTracking()
            .Include(x => x.Time)
            .Where(x => x.TimeId == timeId)
            .OrderBy(x => x.NumeroCamisa)
            .ToArrayAsync(cancellationToken);
    }

    public Task<bool> JerseyNumberExistsAsync(Guid timeId, int numeroCamisa, CancellationToken cancellationToken)
    {
        return _context.Jogadores.AnyAsync(x => x.TimeId == timeId && x.NumeroCamisa == numeroCamisa, cancellationToken);
    }

    public async Task<(int TotalGols, int JogosComGol, int TotalJogosTime)> GetGoalStatsAsync(Guid timeId, string nomeAutor, CancellationToken cancellationToken)
    {
        var gols = await _context.GolsPartida
            .Where(g => g.TimeId == timeId && g.NomeAutor == nomeAutor)
            .ToListAsync(cancellationToken);

        var totalGols = gols.Sum(g => g.QuantidadeGols);
        var jogosComGol = gols.Count;

        var totalJogosTime = await _context.Desafios.CountAsync(
            d => d.Status == DesafioStatus.Finalizado &&
                 (d.TimeCriadorId == timeId || d.TimeDesafianteId == timeId),
            cancellationToken);

        return (totalGols, jogosComGol, totalJogosTime);
    }
}
