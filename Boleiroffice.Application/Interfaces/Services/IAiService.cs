using Boleiroffice.Application.DTOs.Ia;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IAiService
{
    Task<NarracaoResponse> GerarNarracaoAsync(NarracaoRequest request, CancellationToken cancellationToken = default);
}
