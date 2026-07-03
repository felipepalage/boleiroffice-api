using Boleiroffice.Application.DTOs.Jogadores;

namespace Boleiroffice.Application.DTOs.Times;

public sealed class TimeCreateRequest
{
    public string Nome { get; init; } = string.Empty;
    public Guid EmpresaId { get; init; }
    public int Nivel { get; init; }
    public string BairroBase { get; init; } = string.Empty;
    public string? FotoUrl { get; init; }
    public int EscudoShape { get; init; } = 1;
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public string? Cep { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
}

public sealed class TimeUpdateRequest
{
    public string Nome { get; init; } = string.Empty;
    public int Nivel { get; init; }
    public string BairroBase { get; init; } = string.Empty;
    public int EscudoShape { get; init; } = 1;
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public string? Cep { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
}

public sealed class TimeImageUpdateRequest
{
    public string? FotoUrl { get; init; }
}

public sealed class TimeLookupResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? FotoUrl { get; init; }
}

public class TimeResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public Guid EmpresaId { get; init; }
    public string EmpresaNome { get; init; } = string.Empty;
    public string? EmpresaLogoUrl { get; init; }
    public int Nivel { get; init; }
    public string BairroBase { get; init; } = string.Empty;
    public string? FotoUrl { get; init; }
    public int EscudoShape { get; init; }
    public string CorPrimaria { get; init; } = "#DC2626";
    public string CorSecundaria { get; init; } = "#111827";
    public string? Cep { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public DateTime DataCriacao { get; init; }
    public int TotalJogadores { get; init; }
}

public class TimeDetailsResponse : TimeResponse
{
    public IReadOnlyCollection<JogadorResponse> Jogadores { get; init; } = Array.Empty<JogadorResponse>();
}