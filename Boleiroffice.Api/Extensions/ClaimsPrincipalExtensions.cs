using System.Security.Claims;
using Boleiroffice.Application.Common.Models;

namespace Boleiroffice.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser GetCurrentUser(this ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("Usu�rio n�o autenticado.");
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("Claim de usu�rio ausente.");

        var empresaId = user.FindFirstValue("empresa_id")
            ?? throw new UnauthorizedAccessException("Claim de empresa ausente.");

        return new CurrentUser
        {
            UsuarioId = Guid.Parse(userId),
            EmpresaId = Guid.Parse(empresaId),
            Nome = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("name") ?? string.Empty,
            Email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email") ?? string.Empty,
            IsAdmin = user.HasClaim("is_admin", "true")
        };
    }
}
