namespace Boleiroffice.Application.DTOs.Ranking;

public record RivalResponse(
    Guid TimeId,
    string NomeTime,
    int TotalJogos,
    int Vitorias,
    int Derrotas,
    int Empates,
    string? BairroBase);

public record RividadesResponse(
    Guid TimeId,
    IReadOnlyList<RivalResponse> Rivais);
