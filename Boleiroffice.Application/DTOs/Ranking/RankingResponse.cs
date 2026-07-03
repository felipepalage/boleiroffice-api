namespace Boleiroffice.Application.DTOs.Ranking;

public sealed class RankingResponse
{
    public int Posicao { get; set; }
    public Guid TimeId { get; init; }
    public string Time { get; init; } = string.Empty;
    public string Empresa { get; init; } = string.Empty;
    public string? TimeFotoUrl { get; init; }
    public string? EmpresaLogoUrl { get; init; }
    public int EscudoShape { get; init; }
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public int Jogos { get; init; }
    public int Vitorias { get; init; }
    public int Empates { get; init; }
    public int Derrotas { get; init; }
    public int GolsPro { get; init; }
    public int GolsContra { get; init; }
    public int Saldo { get; init; }
    public int Pontos { get; init; }
}

public sealed class ArtilheiroRankingResponse
{
    public int Posicao { get; set; }
    public Guid TimeId { get; init; }
    public string Time { get; init; } = string.Empty;
    public string Empresa { get; init; } = string.Empty;
    public string? TimeFotoUrl { get; init; }
    public string? EmpresaLogoUrl { get; init; }
    public int EscudoShape { get; init; }
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public string NomeAutor { get; init; } = string.Empty;
    public int Gols { get; init; }
    public int JogosComGol { get; init; }
}

public sealed class ReputacaoRankingResponse
{
    public int Posicao { get; set; }
    public Guid TimeId { get; init; }
    public string Time { get; init; } = string.Empty;
    public string Empresa { get; init; } = string.Empty;
    public string? TimeFotoUrl { get; init; }
    public string? EmpresaLogoUrl { get; init; }
    public int EscudoShape { get; init; }
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public int JogosConfirmados { get; init; }
    public int Comparecimentos { get; init; }
    public int CancelamentosTardios { get; init; }
    public int ConfirmacoesRapidas { get; init; }
    public int IndiceConfiabilidade { get; init; }
}
