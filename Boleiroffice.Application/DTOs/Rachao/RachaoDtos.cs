namespace Boleiroffice.Application.DTOs.Rachao;

public sealed record CriarRachaoRequest(DateTime HorarioEvento, int NumeroTimes);

public sealed record ConfirmarPresencaRequest(string Nome, string Empresa);

public sealed record RachaoConfirmacaoResponse(Guid Id, string Nome, string? Empresa);

// Visao do dono (autenticado) - gerencia o evento
public sealed record RachaoEventoResponse(
    Guid Id,
    string Token,
    DateTime HorarioEvento,
    int NumeroTimes,
    bool SorteioFeito,
    IReadOnlyList<RachaoConfirmacaoResponse> Confirmados);

public sealed record TimeSorteadoResponse(string Nome, IReadOnlyList<string> Jogadores);

// Visao publica (link) - sem login
public sealed record RachaoPublicoResponse(
    string Token,
    string EmpresaNome,
    DateTime HorarioEvento,
    int NumeroTimes,
    bool SorteioFeito,
    IReadOnlyList<RachaoConfirmacaoResponse> Confirmados,
    IReadOnlyList<TimeSorteadoResponse> Times);