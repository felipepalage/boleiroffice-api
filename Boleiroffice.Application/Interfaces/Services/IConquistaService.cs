using Boleiroffice.Application.DTOs.Conquista;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IConquistaService
{
    Task<ConquistasTimeResponse> GetConquistasAsync(Guid timeId, CancellationToken cancellationToken = default);
}
