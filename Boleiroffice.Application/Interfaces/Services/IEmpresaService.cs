using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Empresas;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IEmpresaService
{
    Task<EmpresaResponse> CreateAsync(EmpresaCreateRequest request, CancellationToken cancellationToken);
    Task<EmpresaResponse> UpdateLogoAsync(Guid id, EmpresaImageUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<EmpresaResponse> UploadLogoAsync(Guid id, Stream fileStream, string fileName, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<EmpresaDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<EmpresaResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
}
