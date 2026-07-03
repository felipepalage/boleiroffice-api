namespace Boleiroffice.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public AuthenticatedUserResponse Usuario { get; init; } = new();
}
