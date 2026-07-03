using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IReacaoFeedService
{
    Task<ReacoesDesafioResponse> GetByDesafioAsync(Guid desafioId, Guid? minhaEmpresaId, CancellationToken cancellationToken = default);
    Task<ReacoesDesafioResponse> ReagirAsync(Guid desafioId, ReagirFeedRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
