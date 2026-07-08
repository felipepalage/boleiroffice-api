namespace Boleiroffice.Application.DTOs.Auth;

public sealed class RegisterRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string EmpresaNome { get; init; } = string.Empty;
    public string EmpresaCnpj { get; init; } = string.Empty;
    public string EmpresaBairro { get; init; } = string.Empty;
    public string EmpresaCidade { get; init; } = string.Empty;
    public string? EmpresaLogoUrl { get; init; }
    public Guid? IndicadoPorEmpresaId { get; init; }
}