using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class MensagemDesafioRepository : IMensagemDesafioRepository
{
    private readonly ApplicationDbContext _context;

    public MensagemDesafioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MensagemDesafio>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default)
    {
        return await _context.MensagensDesafio
            .Where(x => x.DesafioId == desafioId)
            .OrderBy(x => x.DataEnvio)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MensagemDesafio mensagem, CancellationToken cancellationToken = default)
    {
        mensagem.Id = Guid.NewGuid();
        _context.MensagensDesafio.Add(mensagem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
