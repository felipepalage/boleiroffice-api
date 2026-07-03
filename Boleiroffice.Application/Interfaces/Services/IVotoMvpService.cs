using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Mvp;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IVotoMvpService
{
    Task<VotacaoMvpResponse> GetByDesafioAsync(Guid desafioId, Guid? minhaEmpresaId, CancellationToken cancellationToken = default);
    Task<VotacaoMvpResponse> VotarAsync(Guid desafioId, VotarMvpRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
