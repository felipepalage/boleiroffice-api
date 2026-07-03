namespace Boleiroffice.Application.DTOs.Jogadores;

public sealed class JogadorCreateRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Posicao { get; init; } = string.Empty;
    public int NumeroCamisa { get; init; }
    public Guid TimeId { get; init; }
}

public sealed class JogadorResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Posicao { get; init; } = string.Empty;
    public int NumeroCamisa { get; init; }
    public Guid TimeId { get; init; }
    public string TimeNome { get; init; } = string.Empty;
}

public sealed class JogadorPerfilResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Posicao { get; init; } = string.Empty;
    public int NumeroCamisa { get; init; }
    public Guid TimeId { get; init; }
    public string TimeNome { get; init; } = string.Empty;
    public string NomeEmpresa { get; init; } = string.Empty;
    public int EscudoShape { get; init; }
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public int TotalGols { get; init; }
    public int JogosComGol { get; init; }
    public int TotalJogosTime { get; init; }
}
