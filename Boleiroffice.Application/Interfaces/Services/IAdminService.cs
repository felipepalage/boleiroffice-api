using Boleiroffice.Application.DTOs.Admin;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);
}
