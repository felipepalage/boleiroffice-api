namespace Boleiroffice.Application.DTOs.Relatorio;

public record RelatorioJogoResponse(
    DateOnly DataJogo,
    string HoraJogo,
    string Local,
    string Adversario,
    string Resultado,
    int? GolsPro,
    int? GolsContra,
    string Status);

public record RelatorioTimeResponse(
    Guid TimeId,
    string NomeTime,
    int TotalJogos,
    int Vitorias,
    int Derrotas,
    int Empates,
    int GolsPro,
    int GolsContra,
    IReadOnlyList<RelatorioJogoResponse> Jogos);
