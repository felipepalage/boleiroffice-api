using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Disponibilidade;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IDisponibilidadeService
{
    Task<IReadOnlyList<DisponibilidadeResponse>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BrowseDisponibilidadeItem>> BrowseAsync(string? cidade, string? bairro, int? diaSemana, Guid? meuTimeId, CancellationToken cancellationToken = default);
    Task<DisponibilidadeResponse> CreateAsync(CreateDisponibilidadeRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
    Task<DisponibilidadeResponse> UpdateAsync(Guid id, Guid timeId, DisponibilidadeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid timeId, CancellationToken cancellationToken = default);
    Task DesafiarDeSlotAsync(Guid slotId, DesafiarDeSlotBodyRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default);
}
