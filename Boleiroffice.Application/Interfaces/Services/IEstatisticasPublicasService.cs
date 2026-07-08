using Boleiroffice.Application.DTOs.Ranking;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IEstatisticasPublicasService
{
    Task<EstatisticasPublicasResponse> GetAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<IndicadorResponse>> GetTopIndicadoresAsync(CancellationToken cancellationToken);
}
