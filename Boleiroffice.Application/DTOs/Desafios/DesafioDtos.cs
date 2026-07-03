using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.DTOs.Desafios;

public sealed class DesafioCreateRequest
{
    public Guid TimeCriadorId { get; init; }
    public Guid TimeConvidadoId { get; init; }
    public DateOnly DataJogo { get; init; }
    public TimeOnly HoraJogo { get; init; }
    public string Local { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public int Nivel { get; init; }
}

public sealed class AcceptDesafioRequest
{
    public Guid TimeDesafianteId { get; init; }
}

public sealed class CancelDesafioRequest
{
    public string? Motivo { get; init; }
}

public sealed class RegisterResultRequest
{
    public int PlacarCriador { get; init; }
    public int PlacarDesafiante { get; init; }
}

public sealed class ConfirmResultRequest
{
    public int PlacarCriador { get; init; }
    public int PlacarDesafiante { get; init; }
}

public sealed class GolPartidaRequest
{
    public string NomeAutor { get; init; } = string.Empty;
    public int QuantidadeGols { get; init; }
}

public sealed class RegistrarArtilheirosRequest
{
    public IReadOnlyCollection<GolPartidaRequest> GolsCriador { get; init; } = Array.Empty<GolPartidaRequest>();
    public IReadOnlyCollection<GolPartidaRequest> GolsDesafiante { get; init; } = Array.Empty<GolPartidaRequest>();
}

public sealed class GolPartidaResponse
{
    public Guid Id { get; init; }
    public Guid TimeId { get; init; }
    public string Time { get; init; } = string.Empty;
    public string NomeAutor { get; init; } = string.Empty;
    public int QuantidadeGols { get; init; }
}

public sealed class DesafioResponse
{
    public Guid Id { get; init; }
    public Guid TimeCriadorId { get; init; }
    public string TimeCriador { get; init; } = string.Empty;
    public string EmpresaCriadora { get; init; } = string.Empty;
    public string? TimeCriadorFotoUrl { get; init; }
    public int TimeCriadorEscudoShape { get; init; }
    public string TimeCriadorCorPrimaria { get; init; } = "#DC2626";
    public string TimeCriadorCorSecundaria { get; init; } = "#111827";
    public Guid? TimeDesafianteId { get; init; }
    public string? TimeDesafiante { get; init; }
    public string? EmpresaDesafiante { get; init; }
    public string? TimeDesafianteFotoUrl { get; init; }
    public int? TimeDesafianteEscudoShape { get; init; }
    public string? TimeDesafianteCorPrimaria { get; init; }
    public string? TimeDesafianteCorSecundaria { get; init; }
    public DateOnly DataJogo { get; init; }
    public TimeOnly HoraJogo { get; init; }
    public string Local { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public int Nivel { get; init; }
    public DesafioStatus Status { get; init; }
    public int? PlacarCriador { get; init; }
    public int? PlacarDesafiante { get; init; }
    public int? PlacarCriadorProposto { get; init; }
    public int? PlacarDesafianteProposto { get; init; }
    public Guid? ResultadoPropostoPorTimeId { get; init; }
    public string? ResultadoPropostoPorTime { get; init; }
    public bool ResultadoConfirmadoPeloCriador { get; init; }
    public bool ResultadoConfirmadoPeloDesafiante { get; init; }
    public DateTime? DataPropostaResultado { get; init; }
    public DateTime? DataAceite { get; init; }
    public DateTime? DataCancelamento { get; init; }
    public DateTime? DataResultadoConfirmadoEm { get; init; }
    public IReadOnlyCollection<GolPartidaResponse> Gols { get; init; } = Array.Empty<GolPartidaResponse>();
    public DateTime DataCriacao { get; init; }
}

public sealed class SuggestedChallengeResponse
{
    public Guid TimeId { get; init; }
    public string NomeTime { get; init; } = string.Empty;
    public string Empresa { get; init; } = string.Empty;
    public string BairroBase { get; init; } = string.Empty;
    public int Nivel { get; init; }
    public bool DisponivelNaData { get; init; }
    public int ScoreCompatibilidade { get; init; }
    public string Motivo { get; init; } = string.Empty;
}
