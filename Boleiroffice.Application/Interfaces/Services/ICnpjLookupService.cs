using Boleiroffice.Application.DTOs.Auth;

namespace Boleiroffice.Application.Interfaces.Services;

/// <summary>Consulta dados de uma empresa a partir do CNPJ (fonte externa, best-effort).</summary>
public interface ICnpjLookupService
{
    Task<CnpjInfoResponse?> ConsultarAsync(string cnpj, CancellationToken cancellationToken);
}
