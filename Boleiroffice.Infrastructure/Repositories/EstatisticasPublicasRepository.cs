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

        // Só conta desafios oficiais entre empresas — amistoso/rachão (jogo interno) não entra.
        var jogos = await _context.Desafios
            .CountAsync(x => x.Status == DesafioStatus.Finalizado, cancellationToken);
        var gols = await _context.Desafios
            .Where(x => x.Status == DesafioStatus.Finalizado)
            .SumAsync(x => (x.PlacarCriador ?? 0) + (x.PlacarDesafiante ?? 0), cancellationToken);

        return new EstatisticasPublicasResponse(empresas, times, jogos, gols);
    }

    public async Task<IReadOnlyList<IndicadorResponse>> GetTopIndicadoresAsync(int limite, CancellationToken cancellationToken)
    {
        var counts = await _context.Empresas
            .Where(x => x.IndicadaPorEmpresaId != null)
            .GroupBy(x => x.IndicadaPorEmpresaId!.Value)
            .Select(g => new { EmpresaId = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .Take(limite)
            .ToListAsync(cancellationToken);

        if (counts.Count == 0)
            return Array.Empty<IndicadorResponse>();

        var ids = counts.Select(c => c.EmpresaId).ToList();
        var nomes = await _context.Empresas
            .Where(e => ids.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, e => e.Nome, cancellationToken);

        return counts
            .Select(c => new IndicadorResponse(c.EmpresaId, nomes.TryGetValue(c.EmpresaId, out var n) ? n : "Empresa", c.Total))
            .ToList();
    }
}
