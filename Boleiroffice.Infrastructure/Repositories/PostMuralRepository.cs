using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class PostMuralRepository : IPostMuralRepository
{
    private readonly ApplicationDbContext _context;

    public PostMuralRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PostMural>> GetByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken = default)
    {
        return await _context.PostsMural
            .Include(x => x.Empresa)
            .Where(x => x.EmpresaId == empresaId)
            .OrderByDescending(x => x.DataPublicacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<PostMural?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.PostsMural.FindAsync([id], cancellationToken);

    public async Task AddAsync(PostMural post, CancellationToken cancellationToken = default)
    {
        post.Id = Guid.NewGuid();
        _context.PostsMural.Add(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PostMural post, CancellationToken cancellationToken = default)
    {
        _context.PostsMural.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
