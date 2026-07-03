using Boleiroffice.Application.Common.Models;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IEmpresaRepository
{
    Task AddAsync(Empresa empresa, CancellationToken cancellationToken);
    Task UpdateAsync(Empresa empresa, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string nome, CancellationToken cancellationToken);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken);
    Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);
    Task<Empresa?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Empresa>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken);
}