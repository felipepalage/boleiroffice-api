namespace Boleiroffice.Application.DTOs.Calendario;

public record CalendarioItemResponse(
    Guid DesafioId,
    DateOnly DataJogo,
    string HoraJogo,
    string Local,
    string Bairro,
    string StatusLabel,
    string NomeAdversario,
    bool SouCriador);

public record CalendarioMesResponse(
    int Ano,
    int Mes,
    IReadOnlyList<CalendarioItemResponse> Jogos);
