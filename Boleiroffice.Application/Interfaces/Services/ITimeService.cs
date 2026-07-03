using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Times;

namespace Boleiroffice.Application.Interfaces.Services;

public interface ITimeService
{
    Task<TimeResponse> CreateAsync(TimeCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<TimeResponse> UpdateImageAsync(Guid id, TimeImageUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<TimeResponse> UploadImageAsync(Guid id, Stream fileStream, string fileName, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<TimeDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TimeResponse> UpdateAsync(Guid id, TimeUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<PagedResult<TimeResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
}
