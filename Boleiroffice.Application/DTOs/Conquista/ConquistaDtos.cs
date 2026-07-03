namespace Boleiroffice.Application.DTOs.Conquista;

public record ConquistaResponse(
    string Tipo,
    string Titulo,
    string Descricao,
    string Emoji,
    bool Conquistado,
    DateTime? DataConquista);

public record ConquistasTimeResponse(
    Guid TimeId,
    IReadOnlyList<ConquistaResponse> Conquistas);
