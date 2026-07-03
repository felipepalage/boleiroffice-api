using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Financeiro;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IFinanceiroService
{
    Task<FinanceiroSummaryResponse> GetByTimeAsync(Guid timeId, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task<FinanceiroItemResponse> CreateAsync(Guid timeId, FinanceiroItemRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task<FinanceiroItemResponse> MarcarPagoAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
