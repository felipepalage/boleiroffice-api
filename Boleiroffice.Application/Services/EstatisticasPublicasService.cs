using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Application.Services;

public sealed class EstatisticasPublicasService : IEstatisticasPublicasService
{
    private const string CacheKey = "estatisticas:publicas";
    private readonly IEstatisticasPublicasRepository _repository;
    private readonly ICacheService _cacheService;

    public EstatisticasPublicasService(IEstatisticasPublicasRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<EstatisticasPublicasResponse> GetAsync(CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync<EstatisticasPublicasResponse>(CacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var stats = await _repository.GetAsync(cancellationToken);
        await _cacheService.SetAsync(CacheKey, stats, TimeSpan.FromMinutes(2), cancellationToken);
        return stats;
    }
}
