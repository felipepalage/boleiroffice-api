using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class RachaoEventoRepository : IRachaoEventoRepository
{
    private readonly ApplicationDbContext _context;

    public RachaoEventoRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(RachaoEvento evento, CancellationToken cancellationToken)
    {
        await _context.RachaoEventos.AddAsync(evento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddConfirmacaoAsync(RachaoConfirmacao confirmacao, CancellationToken cancellationToken)
    {
        await _context.RachaoConfirmacoes.AddAsync(confirmacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveConfirmacaoAsync(RachaoConfirmacao confirmacao, CancellationToken cancellationToken)
    {
        _context.RachaoConfirmacoes.Remove(confirmacao);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<RachaoEvento?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        => _context.RachaoEventos
            .Include(x => x.Confirmacoes)
            .Include(x => x.Empresa)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public Task<RachaoEvento?> GetAtivoByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken)
        => _context.RachaoEventos
            .Include(x => x.Confirmacoes)
            .Where(x => x.EmpresaId == empresaId)
            .OrderByDescending(x => x.DataCriacao)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<RachaoEvento>> GetPendentesSorteioAsync(DateTime agora, CancellationToken cancellationToken)
    {
        var limite = agora.AddHours(2); // sorteia quando faltam <= 2h (ou já passou)
        return await _context.RachaoEventos
            .Include(x => x.Confirmacoes)
            .Where(x => !x.SorteioFeito && x.HorarioEvento <= limite)
            .ToListAsync(cancellationToken);
    }

    public Task SaveAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

    public Task<int> RemoverAntigosAsync(DateTime cutoff, CancellationToken cancellationToken)
        => _context.RachaoEventos.Where(x => x.DataCriacao < cutoff).ExecuteDeleteAsync(cancellationToken);
}
