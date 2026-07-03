using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Conquista;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class ConquistaService : IConquistaService
{
    private readonly IDesafioRepository _desafioRepo;

    public ConquistaService(IDesafioRepository desafioRepo)
    {
        _desafioRepo = desafioRepo;
    }

    public async Task<ConquistasTimeResponse> GetConquistasAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        var paged = await _desafioRepo.GetByTimeIdAsync(
            timeId, new PaginationParameters { PageSize = 500 }, cancellationToken);

        var finalizados = paged.Items
            .Where(d => d.Status == DesafioStatus.Finalizado)
            .OrderBy(d => d.DataJogo)
            .ToList();

        var conquistas = new List<ConquistaResponse>
        {
            ComputeEstreante(timeId, finalizados),
            ComputeVeterano(timeId, finalizados),
            ComputeCentenario(timeId, finalizados),
            ComputeInvicto(timeId, finalizados),
            ComputeDominante(timeId, finalizados),
            ComputeGoleada(timeId, finalizados),
        };

        return new ConquistasTimeResponse(timeId, conquistas);
    }

    private static ConquistaResponse ComputeEstreante(Guid timeId, List<Desafio> finalizados)
    {
        var conquistado = finalizados.Count >= 1;
        return new ConquistaResponse(
            "ESTREANTE", "Estreante", "Dispute sua primeira partida", "🎬",
            conquistado, conquistado ? finalizados.FirstOrDefault()?.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }

    private static ConquistaResponse ComputeGoleada(Guid timeId, List<Desafio> finalizados)
    {
        var goleada = finalizados.FirstOrDefault(d =>
        {
            bool souCriador = d.TimeCriadorId == timeId;
            var meu = souCriador ? d.PlacarCriador : d.PlacarDesafiante;
            var dele = souCriador ? d.PlacarDesafiante : d.PlacarCriador;
            return (meu - dele) >= 4;
        });

        var conquistado = goleada is not null;
        return new ConquistaResponse(
            "GOLEADA", "Goleada", "Vença uma partida por 4 gols ou mais de diferença", "💥",
            conquistado, conquistado ? goleada!.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }

    private static ConquistaResponse ComputeVeterano(Guid timeId, List<Desafio> finalizados)
    {
        var conquistado = finalizados.Count >= 10;
        return new ConquistaResponse(
            "VETERANO", "Veterano", "Dispute 10 partidas finalizadas", "🏆",
            conquistado, conquistado ? finalizados.ElementAtOrDefault(9)?.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }

    private static ConquistaResponse ComputeCentenario(Guid timeId, List<Desafio> finalizados)
    {
        var conquistado = finalizados.Count >= 50;
        return new ConquistaResponse(
            "CENTENARIO", "Centenário", "Dispute 50 partidas finalizadas", "💯",
            conquistado, conquistado ? finalizados.ElementAtOrDefault(49)?.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }

    private static ConquistaResponse ComputeInvicto(Guid timeId, List<Desafio> finalizados)
    {
        var ultimos = finalizados.TakeLast(5).ToList();
        var conquistado = ultimos.Count == 5 && ultimos.All(d =>
        {
            bool souCriador = d.TimeCriadorId == timeId;
            var meu = souCriador ? d.PlacarCriador : d.PlacarDesafiante;
            var dele = souCriador ? d.PlacarDesafiante : d.PlacarCriador;
            return meu >= dele;
        });

        return new ConquistaResponse(
            "INVICTO", "Invicto", "Vença ou empate os últimos 5 jogos seguidos", "🔥",
            conquistado, conquistado ? finalizados.LastOrDefault()?.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }

    private static ConquistaResponse ComputeDominante(Guid timeId, List<Desafio> finalizados)
    {
        var ultimos = finalizados.TakeLast(10).ToList();
        var vitorias = ultimos.Count(d =>
        {
            bool souCriador = d.TimeCriadorId == timeId;
            var meu = souCriador ? d.PlacarCriador : d.PlacarDesafiante;
            var dele = souCriador ? d.PlacarDesafiante : d.PlacarCriador;
            return meu > dele;
        });

        var conquistado = ultimos.Count >= 10 && vitorias >= 7;
        return new ConquistaResponse(
            "DOMINANTE", "Dominante", "Vença 7 dos últimos 10 jogos", "⚡",
            conquistado, conquistado ? finalizados.LastOrDefault()?.DataJogo.ToDateTime(TimeOnly.MinValue) : null);
    }
}
