using Boleiroffice.Application.DTOs.Temporada;

namespace Boleiroffice.Application.Interfaces.Services;

public interface ITemporadaService
{
    Task<IReadOnlyList<TemporadaResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TemporadaResponse> CreateAsync(TemporadaRequest request, CancellationToken cancellationToken = default);
    Task<TemporadaResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
