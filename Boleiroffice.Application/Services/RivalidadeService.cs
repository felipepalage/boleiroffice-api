using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Ranking;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class RivalidadeService : IRivalidadeService
{
    private readonly IDesafioRepository _desafioRepo;
    private readonly ITimeRepository _timeRepo;

    public RivalidadeService(IDesafioRepository desafioRepo, ITimeRepository timeRepo)
    {
        _desafioRepo = desafioRepo;
        _timeRepo = timeRepo;
    }

    public async Task<RividadesResponse> GetRivaisAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        var paged = await _desafioRepo.GetByTimeIdAsync(
            timeId, new PaginationParameters { PageSize = 500 }, cancellationToken);

        var finalizados = paged.Items.Where(d => d.Status == DesafioStatus.Finalizado).ToList();

        var grupos = finalizados
            .GroupBy(d => d.TimeCriadorId == timeId ? d.TimeDesafianteId ?? Guid.Empty : d.TimeCriadorId)
            .Where(g => g.Key != Guid.Empty)
            .Select(g =>
            {
                int v = 0, d = 0, e = 0;
                foreach (var jogo in g)
                {
                    bool souCriador = jogo.TimeCriadorId == timeId;
                    var meu = souCriador ? jogo.PlacarCriador : jogo.PlacarDesafiante;
                    var dele = souCriador ? jogo.PlacarDesafiante : jogo.PlacarCriador;
                    if (meu > dele) v++;
                    else if (meu < dele) d++;
                    else e++;
                }

                var time = g.First().TimeCriadorId == timeId
                    ? g.First().TimeDesafiante
                    : g.First().TimeCriador;

                return new { TimeId = g.Key, Total = g.Count(), V = v, D = d, E = e, Time = time };
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();

        var rivais = new List<RivalResponse>();
        foreach (var grupo in grupos)
        {
            var time = grupo.Time ?? await _timeRepo.GetByIdAsync(grupo.TimeId, cancellationToken);
            rivais.Add(new RivalResponse(
                TimeId: grupo.TimeId,
                NomeTime: time?.Nome ?? "Desconhecido",
                TotalJogos: grupo.Total,
                Vitorias: grupo.V,
                Derrotas: grupo.D,
                Empates: grupo.E,
                BairroBase: time?.BairroBase));
        }

        return new RividadesResponse(timeId, rivais);
    }
}
