using Boleiroffice.Application.DTOs.Admin;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IAdminRepository _repo;

    public AdminService(IAdminRepository repo)
    {
        _repo = repo;
    }

    public async Task<AdminStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var empresas = await _repo.CountEmpresasAsync(cancellationToken);
        var desafios = await _repo.CountDesafiosAsync(cancellationToken);
        var usuarios = await _repo.CountUsuariosAsync(cancellationToken);
        var semana = await _repo.CountDesafiosLastDaysAsync(7, cancellationToken);
        var mes = await _repo.CountDesafiosLastDaysAsync(30, cancellationToken);

        return new AdminStatsResponse(empresas, desafios, usuarios, semana, mes);
    }
}
