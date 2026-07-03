using Boleiroffice.Application.DTOs.Relatorio;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IRelatorioService
{
    Task<RelatorioTimeResponse> GetRelatorioAsync(Guid timeId, CancellationToken cancellationToken = default);
}
