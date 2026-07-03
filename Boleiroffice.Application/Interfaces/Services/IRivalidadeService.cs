using Boleiroffice.Application.DTOs.Ranking;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IRivalidadeService
{
    Task<RividadesResponse> GetRivaisAsync(Guid timeId, CancellationToken cancellationToken = default);
}
