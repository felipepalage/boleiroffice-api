using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Relatorio;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class RelatorioService : IRelatorioService
{
    private readonly IDesafioRepository _desafioRepo;
    private readonly ITimeRepository _timeRepo;

    public RelatorioService(IDesafioRepository desafioRepo, ITimeRepository timeRepo)
    {
        _desafioRepo = desafioRepo;
        _timeRepo = timeRepo;
    }

    public async Task<RelatorioTimeResponse> GetRelatorioAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        var time = await _timeRepo.GetByIdAsync(timeId, cancellationToken)
            ?? throw new NotFoundException("Time não encontrado.");

        var paged = await _desafioRepo.GetByTimeIdAsync(
            timeId, new PaginationParameters { PageSize = 500 }, cancellationToken);

        var jogos = paged.Items
            .Where(d => d.Status != DesafioStatus.Cancelado)
            .OrderByDescending(d => d.DataJogo)
            .Select(d =>
            {
                bool souCriador = d.TimeCriadorId == timeId;
                var meu = souCriador ? d.PlacarCriador : d.PlacarDesafiante;
                var dele = souCriador ? d.PlacarDesafiante : d.PlacarCriador;
                var adversario = souCriador ? d.TimeDesafiante?.Nome : d.TimeCriador?.Nome;

                string resultado = d.Status == DesafioStatus.Finalizado
                    ? meu > dele ? "V" : meu < dele ? "D" : "E"
                    : "-";

                return new RelatorioJogoResponse(
                    DataJogo: d.DataJogo,
                    HoraJogo: d.HoraJogo.ToString("HH:mm"),
                    Local: d.Local,
                    Adversario: adversario ?? "Desconhecido",
                    Resultado: resultado,
                    GolsPro: meu,
                    GolsContra: dele,
                    Status: StatusToLabel(d.Status));
            })
            .ToList();

        var finalizados = jogos.Where(j => j.Status == "Finalizado").ToList();
        int v = finalizados.Count(j => j.Resultado == "V");
        int de = finalizados.Count(j => j.Resultado == "D");
        int e = finalizados.Count(j => j.Resultado == "E");
        int golsPro = finalizados.Sum(j => j.GolsPro ?? 0);
        int golsContra = finalizados.Sum(j => j.GolsContra ?? 0);

        return new RelatorioTimeResponse(
            TimeId: timeId,
            NomeTime: time.Nome,
            TotalJogos: finalizados.Count,
            Vitorias: v,
            Derrotas: de,
            Empates: e,
            GolsPro: golsPro,
            GolsContra: golsContra,
            Jogos: jogos);
    }

    private static string StatusToLabel(DesafioStatus s) => s switch
    {
        DesafioStatus.Aberto => "Aberto",
        DesafioStatus.Aceito => "Confirmado",
        DesafioStatus.ResultadoPendente => "Aguardando resultado",
        DesafioStatus.Finalizado => "Finalizado",
        DesafioStatus.Cancelado => "Cancelado",
        _ => s.ToString()
    };
}
