using Boleiroffice.Application.Common.Security;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    TokenResult GenerateToken(Usuario usuario, string empresaNome);
}
