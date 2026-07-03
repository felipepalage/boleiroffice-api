using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IDesafioService
{
    Task<DesafioResponse> AcceptAsync(Guid id, AcceptDesafioRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<DesafioResponse> CancelAsync(Guid id, CancelDesafioRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<DesafioResponse> ConfirmResultAsync(Guid id, ConfirmResultRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<DesafioResponse> CreateAsync(DesafioCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<DesafioResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<DesafioResponse>> GetByTimeIdAsync(Guid timeId, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<DesafioResponse>> GetOpenAsync(string? bairro, DateOnly? dataJogo, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<PagedResult<SuggestedChallengeResponse>> GetSuggestedAsync(Guid timeId, DateOnly dataJogo, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<DesafioResponse> RegisterResultAsync(Guid id, RegisterResultRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task<DesafioResponse> RegisterScorersAsync(Guid id, RegistrarArtilheirosRequest request, CurrentUser currentUser, CancellationToken cancellationToken);
    Task AutoFinalizeExpiredAsync(CancellationToken cancellationToken);
}
