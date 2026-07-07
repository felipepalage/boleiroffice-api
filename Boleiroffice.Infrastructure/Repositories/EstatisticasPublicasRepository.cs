using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Enums;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class EstatisticasPublicasRepository : IEstatisticasPublicasRepository
{
    private readonly ApplicationDbContext _context;

    public EstatisticasPublicasRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EstatisticasPublicasResponse> GetAsync(CancellationToken cancellationToken)
    {
        var empresas = await _context.Empresas.CountAsync(cancellationToken);
        var times = await _context.Times.CountAsync(cancellationToken);

        var jogosDesafio = await _context.Desafios
            .CountAsync(x => x.Status == DesafioStatus.Finalizado, cancellationToken);
        var jogosAmistoso = await _context.PartidasAmistoso.CountAsync(cancellationToken);

        var golsAmistoso = await _context.GolsAmistoso.CountAsync(cancellationToken);
        var golsDesafio = await _context.Desafios
            .Where(x => x.Status == DesafioStatus.Finalizado)
            .SumAsync(x => (x.PlacarCriador ?? 0) + (x.PlacarDesafiante ?? 0), cancellationToken);

        return new EstatisticasPublicasResponse(
            empresas,
            times,
            jogosDesafio + jogosAmistoso,
            golsAmistoso + golsDesafio);
    }
}
