using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _context.Usuarios.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(x => x.Empresa)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
