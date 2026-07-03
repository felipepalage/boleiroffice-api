using Boleiroffice.Application.DTOs.Times;

namespace Boleiroffice.Application.DTOs.Empresas;

public sealed class EmpresaCreateRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public string Cidade { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
}

public sealed class EmpresaImageUpdateRequest
{
    public string? LogoUrl { get; init; }
}

public class EmpresaResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public string Cidade { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public DateTime DataCriacao { get; init; }
}

public class EmpresaDetailsResponse : EmpresaResponse
{
    public IReadOnlyCollection<TimeLookupResponse> Times { get; init; } = Array.Empty<TimeLookupResponse>();
}