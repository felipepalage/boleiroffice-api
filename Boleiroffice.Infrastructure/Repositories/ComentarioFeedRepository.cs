using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class ComentarioFeedRepository : IComentarioFeedRepository
{
    private readonly ApplicationDbContext _context;

    public ComentarioFeedRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ComentarioFeed>> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken = default)
    {
        return await _context.ComentariosFeed
            .Where(x => x.DesafioId == desafioId)
            .OrderBy(x => x.DataComentario)
            .ToListAsync(cancellationToken);
    }

    public async Task<ComentarioFeed?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ComentariosFeed.FindAsync([id], cancellationToken);

    public async Task AddAsync(ComentarioFeed comentario, CancellationToken cancellationToken = default)
    {
        comentario.Id = Guid.NewGuid();
        _context.ComentariosFeed.Add(comentario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ComentarioFeed comentario, CancellationToken cancellationToken = default)
    {
        _context.ComentariosFeed.Remove(comentario);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
