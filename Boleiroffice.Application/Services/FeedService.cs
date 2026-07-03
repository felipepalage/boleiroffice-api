using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Application.Services;

public sealed class FeedService : IFeedService
{
    private readonly ICacheService _cacheService;
    private readonly IDesafioRepository _desafioRepository;

    public FeedService(IDesafioRepository desafioRepository, ICacheService cacheService)
    {
        _desafioRepository = desafioRepository;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<FeedJogoResponse>> GetJogosAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var cacheKey = $"feed:jogos:{normalized.Page}:{normalized.PageSize}";
        var cached = await _cacheService.GetAsync<PagedResult<FeedJogoResponse>>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var response = await _desafioRepository.GetFeedAsync(normalized, cancellationToken);
        DecorateEditorial(response.Items);
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2), cancellationToken);

        return response;
    }

    private static void DecorateEditorial(IReadOnlyCollection<FeedJogoResponse> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        var jogoDaRodada = items
            .OrderByDescending(GetEditorialScore)
            .ThenByDescending(x => x.DataJogo)
            .ThenByDescending(x => x.HoraJogo)
            .First();

        var destaqueDoDia = items
            .OrderByDescending(x => x.DataJogo)
            .ThenByDescending(x => x.HoraJogo)
            .First();

        foreach (var item in items)
        {
            item.JogoDaRodada = item.Id == jogoDaRodada.Id;
            item.DestaqueDoDia = item.Id == destaqueDoDia.Id;

            if (item.JogoDaRodada)
            {
                item.ChamadaEditorial = "Jogo da rodada: " + item.ChamadaEditorial;
            }
            else if (item.DestaqueDoDia)
            {
                item.ChamadaEditorial = "Destaque do dia: " + item.ChamadaEditorial;
            }
        }
    }

    private static int GetEditorialScore(FeedJogoResponse item)
    {
        var gols = (item.PlacarCriador ?? 0) + (item.PlacarDesafiante ?? 0);
        var saldo = Math.Abs((item.PlacarCriador ?? 0) - (item.PlacarDesafiante ?? 0));
        var equilibrio = saldo == 0 ? 8 : saldo == 1 ? 6 : 0;
        return (gols * 3) + equilibrio;
    }
}
