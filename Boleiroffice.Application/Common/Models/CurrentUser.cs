namespace Boleiroffice.Application.Common.Models;

public sealed class CurrentUser
{
    public Guid UsuarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsAdmin { get; init; } = false;
}
