using Boleiroffice.Application.DTOs.Ranking;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IEstatisticasPublicasRepository
{
    Task<EstatisticasPublicasResponse> GetAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<IndicadorResponse>> GetTopIndicadoresAsync(int limite, CancellationToken cancellationToken);
}
