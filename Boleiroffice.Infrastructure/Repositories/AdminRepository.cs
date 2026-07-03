using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> CountEmpresasAsync(CancellationToken cancellationToken = default)
        => _context.Empresas.CountAsync(cancellationToken);

    public Task<int> CountDesafiosAsync(CancellationToken cancellationToken = default)
        => _context.Desafios.CountAsync(cancellationToken);

    public Task<int> CountUsuariosAsync(CancellationToken cancellationToken = default)
        => _context.Usuarios.CountAsync(cancellationToken);

    public Task<int> CountDesafiosLastDaysAsync(int days, CancellationToken cancellationToken = default)
    {
        var desde = DateTime.UtcNow.AddDays(-days);
        return _context.Desafios.CountAsync(x => x.DataCriacao >= desde, cancellationToken);
    }
}
