namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<int> CountEmpresasAsync(CancellationToken cancellationToken = default);
    Task<int> CountDesafiosAsync(CancellationToken cancellationToken = default);
    Task<int> CountUsuariosAsync(CancellationToken cancellationToken = default);
    Task<int> CountDesafiosLastDaysAsync(int days, CancellationToken cancellationToken = default);
}
