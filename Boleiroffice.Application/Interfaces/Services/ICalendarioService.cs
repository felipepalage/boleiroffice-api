using Boleiroffice.Application.DTOs.Calendario;

namespace Boleiroffice.Application.Interfaces.Services;

public interface ICalendarioService
{
    Task<CalendarioMesResponse> GetMesAsync(Guid timeId, int ano, int mes, CancellationToken cancellationToken = default);
}
