using Boleiroffice.Application.DTOs.Rachao;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IRachaoEventoService
{
    Task<RachaoEventoResponse> CriarAsync(Guid empresaId, CriarRachaoRequest request, CancellationToken cancellationToken);
    Task<RachaoEventoResponse?> GetAtivoAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<RachaoPublicoResponse?> GetPublicoAsync(string token, CancellationToken cancellationToken);
    Task<RachaoPublicoResponse?> ConfirmarAsync(string token, ConfirmarPresencaRequest request, CancellationToken cancellationToken);
    Task<RachaoPublicoResponse?> DesistirAsync(string token, DesistirPresencaRequest request, CancellationToken cancellationToken);

    // Chamados pelo agendador (background service)
    Task<int> SortearPendentesAsync(CancellationToken cancellationToken);
    Task<int> LimparAntigosAsync(int dias, CancellationToken cancellationToken);
}
