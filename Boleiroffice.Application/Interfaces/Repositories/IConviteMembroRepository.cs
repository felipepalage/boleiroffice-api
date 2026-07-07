using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IConviteMembroRepository
{
    Task AddAsync(ConviteMembro convite, CancellationToken cancellationToken);
    Task<ConviteMembro?> GetValidoByTokenAsync(string token, DateTime agora, CancellationToken cancellationToken);
}
