namespace Boleiroffice.Application.DTOs.Auth;

public sealed class LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}
