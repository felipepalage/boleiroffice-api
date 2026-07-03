using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.DTOs.Feed;

public sealed class FeedJogoResponse
{
    public Guid Id { get; init; }
    public DateOnly DataJogo { get; init; }
    public TimeOnly HoraJogo { get; init; }
    public string Local { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public string TimeCriador { get; init; } = string.Empty;
    public string EmpresaCriadora { get; init; } = string.Empty;
    public string? TimeCriadorFotoUrl { get; init; }
    public string? EmpresaCriadoraLogoUrl { get; init; }
    public int TimeCriadorEscudoShape { get; init; }
    public string TimeCriadorCorPrimaria { get; init; } = "#DC2626";
    public string TimeCriadorCorSecundaria { get; init; } = "#111827";
    public string TimeDesafiante { get; init; } = string.Empty;
    public string EmpresaDesafiante { get; init; } = string.Empty;
    public string? TimeDesafianteFotoUrl { get; init; }
    public string? EmpresaDesafianteLogoUrl { get; init; }
    public int TimeDesafianteEscudoShape { get; init; }
    public string TimeDesafianteCorPrimaria { get; init; } = "#DC2626";
    public string TimeDesafianteCorSecundaria { get; init; } = "#111827";
    public string Manchete { get; set; } = string.Empty;
    public string Resumo { get; set; } = string.Empty;
    public string ChamadaEditorial { get; set; } = string.Empty;
    public string? ArtilheirosResumo { get; set; }
    public bool JogoDaRodada { get; set; }
    public bool DestaqueDoDia { get; set; }
    public int? PlacarCriador { get; init; }
    public int? PlacarDesafiante { get; init; }
    public DesafioStatus Status { get; init; }
}
