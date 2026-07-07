using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Boleiroffice.Infrastructure.Repositories;

public sealed class ConviteMembroRepository : IConviteMembroRepository
{
    private readonly ApplicationDbContext _context;

    public ConviteMembroRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ConviteMembro convite, CancellationToken cancellationToken)
    {
        await _context.ConvitesMembro.AddAsync(convite, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<ConviteMembro?> GetValidoByTokenAsync(string token, DateTime agora, CancellationToken cancellationToken)
        => _context.ConvitesMembro
            .Include(x => x.Empresa)
            .FirstOrDefaultAsync(x => x.Token == token && x.ExpiraEm > agora, cancellationToken);
}
