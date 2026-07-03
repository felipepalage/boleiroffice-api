namespace Boleiroffice.Application.DTOs.Auth;

public sealed class AuthenticatedUserResponse
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid EmpresaId { get; init; }
    public string EmpresaNome { get; init; } = string.Empty;
    public bool IsAdmin { get; init; }
}
