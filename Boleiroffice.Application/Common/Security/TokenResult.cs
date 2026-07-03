namespace Boleiroffice.Application.Common.Security;

public sealed class TokenResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
