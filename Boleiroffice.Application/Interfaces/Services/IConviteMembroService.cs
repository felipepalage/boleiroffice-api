using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.DTOs.Convites;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IConviteMembroService
{
    Task<ConviteResponse> CriarAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<ConviteInfoResponse?> GetInfoAsync(string token, CancellationToken cancellationToken);
    Task<AuthResponse> AceitarAsync(string token, AceitarConviteRequest request, CancellationToken cancellationToken);
}
