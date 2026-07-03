using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class ConfirmacaoPresencaRepository : IConfirmacaoPresencaRepository
{
    private readonly ApplicationDbContext _context;

    public ConfirmacaoPresencaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConfirmacaoPresenca?> GetAsync(Guid jogadorId, Guid desafioId, CancellationToken cancellationToken)
    {
        return await _context.ConfirmacoesPresenca
            .Include(x => x.Jogador)
            .FirstOrDefaultAsync(x => x.JogadorId == jogadorId && x.DesafioId == desafioId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ConfirmacaoPresenca>> GetByDesafioIdAsync(Guid desafioId, CancellationToken cancellationToken)
    {
        return await _context.ConfirmacoesPresenca
            .AsNoTracking()
            .Include(x => x.Jogador)
            .Where(x => x.DesafioId == desafioId)
            .OrderBy(x => x.Jogador!.NumeroCamisa)
            .ToArrayAsync(cancellationToken);
    }

    public async Task UpsertAsync(ConfirmacaoPresenca confirmacao, CancellationToken cancellationToken)
    {
        if (confirmacao.Id == Guid.Empty)
        {
            await _context.ConfirmacoesPresenca.AddAsync(confirmacao, cancellationToken);
        }
        else
        {
            _context.ConfirmacoesPresenca.Update(confirmacao);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
