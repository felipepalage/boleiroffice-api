using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Quadras;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IQuadraService
{
    Task<QuadraResponse> CreateAsync(QuadraCreateRequest request, CancellationToken cancellationToken);
    Task<QuadraResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<QuadraResponse>> GetPagedAsync(string? bairro, PaginationParameters pagination, CancellationToken cancellationToken);
    Task<QuadraResponse> AvaliarAsync(Guid id, AvaliacaoCreateRequest request, Guid empresaId, CancellationToken cancellationToken);
}
