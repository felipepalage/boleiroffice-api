using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class AmistosoRepository : IAmistosoRepository
{
    private readonly ApplicationDbContext _context;

    public AmistosoRepository(ApplicationDbContext context) => _context = context;

    // ---------- Elenco ----------

    public async Task<IReadOnlyList<JogadorAmistoso>> GetJogadoresAsync(Guid empresaId, CancellationToken cancellationToken)
        => await _context.JogadoresAmistoso
            .AsNoTracking()
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

    public Task<JogadorAmistoso?> GetJogadorByIdAsync(Guid id, Guid empresaId, CancellationToken cancellationToken)
        => _context.JogadoresAmistoso.FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == empresaId, cancellationToken);

    public async Task AddJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken)
    {
        await _context.JogadoresAmistoso.AddAsync(jogador, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken)
    {
        _context.JogadoresAmistoso.Update(jogador);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken)
    {
        _context.JogadoresAmistoso.Remove(jogador);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ZerarPagamentosAsync(Guid empresaId, CancellationToken cancellationToken)
    {
        await _context.JogadoresAmistoso
            .Where(x => x.EmpresaId == empresaId && x.PagouMensalidade)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.PagouMensalidade, false), cancellationToken);
    }

    // ---------- Sorteio / Times ----------

    public async Task<IReadOnlyList<TimeAmistoso>> GetTimesAsync(Guid empresaId, CancellationToken cancellationToken)
        => await _context.TimesAmistoso
            .AsNoTracking()
            .Include(x => x.Jogadores)
            .Where(x => x.EmpresaId == empresaId)
            .OrderBy(x => x.Ordem)
            .ToListAsync(cancellationToken);

    public async Task ReplaceTimesAsync(Guid empresaId, IReadOnlyList<TimeAmistoso> novosTimes, CancellationToken cancellationToken)
    {
        var antigos = await _context.TimesAmistoso.Where(x => x.EmpresaId == empresaId).ToListAsync(cancellationToken);
        _context.TimesAmistoso.RemoveRange(antigos); // cascade remove nos jogadores
        await _context.TimesAmistoso.AddRangeAsync(novosTimes, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ---------- Partidas ----------

    public async Task AddPartidaAsync(PartidaAmistoso partida, CancellationToken cancellationToken)
    {
        await _context.PartidasAmistoso.AddAsync(partida, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<PartidaAmistoso?> GetPartidaByIdAsync(Guid id, Guid empresaId, CancellationToken cancellationToken)
        => _context.PartidasAmistoso
            .Include(x => x.Gols)
            .FirstOrDefaultAsync(x => x.Id == id && x.EmpresaId == empresaId, cancellationToken);

    public async Task AddGolAsync(GolAmistoso gol, CancellationToken cancellationToken)
    {
        // INSERT explícito. Como a PK Guid é gerada no cliente, adicionar o gol só pela
        // coleção rastreada faria o EF marcá-lo como Modified (UPDATE de linha inexistente
        // -> DbUpdateConcurrencyException 500). AddAsync força o estado Added.
        // O score da partida (rastreada) é persistido no mesmo SaveChanges.
        await _context.GolsAmistoso.AddAsync(gol, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveGolAsync(GolAmistoso gol, CancellationToken cancellationToken)
    {
        _context.GolsAmistoso.Remove(gol);
        // O placar da partida (rastreada) é persistido no mesmo SaveChanges.
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePartidaAsync(PartidaAmistoso partida, CancellationToken cancellationToken)
    {
        // A partida chega rastreada (GetPartidaByIdAsync); alterações escalares (ex.: finalizar)
        // são detectadas pelo change tracking. Não usar DbSet.Update em grafo rastreado.
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<PartidaAmistoso>> GetPartidasPagedAsync(Guid empresaId, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.PartidasAmistoso
            .AsNoTracking()
            .Include(x => x.Gols)
            .Where(x => x.EmpresaId == empresaId && x.Finalizada)
            .OrderByDescending(x => x.DataInicio);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);

        return PagedResult<PartidaAmistoso>.Create(items, pagination.Page, pagination.PageSize, total);
    }

    // ---------- Gols / ranking / resumo ----------

    public async Task<IReadOnlyList<GolAmistoso>> GetGolsByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken)
        => await _context.GolsAmistoso
            .AsNoTracking()
            .Where(x => x.EmpresaId == empresaId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PartidaAmistoso>> GetPartidasFinalizadasNoDiaAsync(Guid empresaId, DateOnly dia, CancellationToken cancellationToken)
    {
        var inicio = dia.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var fim = inicio.AddDays(1);
        return await _context.PartidasAmistoso
            .AsNoTracking()
            .Include(x => x.Gols)
            .Where(x => x.EmpresaId == empresaId && x.Finalizada && x.DataInicio >= inicio && x.DataInicio < fim)
            .ToListAsync(cancellationToken);
    }
}
