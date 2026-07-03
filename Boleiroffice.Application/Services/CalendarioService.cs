using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Calendario;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class CalendarioService : ICalendarioService
{
    private readonly IDesafioRepository _desafioRepo;
    private readonly ITimeRepository _timeRepo;

    public CalendarioService(IDesafioRepository desafioRepo, ITimeRepository timeRepo)
    {
        _desafioRepo = desafioRepo;
        _timeRepo = timeRepo;
    }

    public async Task<CalendarioMesResponse> GetMesAsync(
        Guid timeId, int ano, int mes, CancellationToken cancellationToken = default)
    {
        var paged = await _desafioRepo.GetByTimeIdAsync(
            timeId, new PaginationParameters { PageSize = 500 }, cancellationToken);

        var jogos = paged.Items
            .Where(d => d.DataJogo.Year == ano && d.DataJogo.Month == mes && d.Status != DesafioStatus.Cancelado)
            .OrderBy(d => d.DataJogo)
            .Select(d =>
            {
                var souCriador = d.TimeCriadorId == timeId;
                var nomeAdversario = souCriador
                    ? d.TimeDesafiante?.Nome ?? "Adversário"
                    : d.TimeCriador?.Nome ?? "Adversário";

                return new CalendarioItemResponse(
                    DesafioId: d.Id,
                    DataJogo: d.DataJogo,
                    HoraJogo: d.HoraJogo.ToString("HH:mm"),
                    Local: d.Local,
                    Bairro: d.Bairro,
                    StatusLabel: StatusToLabel(d.Status),
                    NomeAdversario: nomeAdversario,
                    SouCriador: souCriador);
            })
            .ToList();

        return new CalendarioMesResponse(ano, mes, jogos);
    }

    private static string StatusToLabel(DesafioStatus status) => status switch
    {
        DesafioStatus.Aberto => "Aberto",
        DesafioStatus.Aceito => "Confirmado",
        DesafioStatus.ResultadoPendente => "Aguardando resultado",
        DesafioStatus.Finalizado => "Finalizado",
        DesafioStatus.Cancelado => "Cancelado",
        _ => status.ToString()
    };
}
