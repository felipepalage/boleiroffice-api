namespace Boleiroffice.Application.DTOs.Rachao;

public sealed record CriarRachaoRequest(DateTime HorarioEvento, int NumeroTimes);

public sealed record ConfirmarPresencaRequest(string Nome);

public sealed record RachaoConfirmacaoResponse(Guid Id, string Nome);

// Visão do dono (autenticado) — gerencia o evento
public sealed record RachaoEventoResponse(
    Guid Id,
    string Token,
    DateTime HorarioEvento,
    int NumeroTimes,
    bool SorteioFeito,
    IReadOnlyList<RachaoConfirmacaoResponse> Confirmados);

public sealed record TimeSorteadoResponse(string Nome, IReadOnlyList<string> Jogadores);

// Visão pública (link) — sem login
public sealed record RachaoPublicoResponse(
    string Token,
    string EmpresaNome,
    DateTime HorarioEvento,
    int NumeroTimes,
    bool SorteioFeito,
    IReadOnlyList<string> Confirmados,
    IReadOnlyList<TimeSorteadoResponse> Times);
