namespace Boleiroffice.Application.DTOs.Ia;

public sealed record NarracaoRequest(
    string Titulo,
    string Placar,
    IReadOnlyList<string> Artilheiros,
    string? Contexto);

public sealed record NarracaoResponse(string Texto);
