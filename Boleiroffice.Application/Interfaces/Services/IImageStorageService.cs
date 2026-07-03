namespace Boleiroffice.Application.Interfaces.Services;

public interface IImageStorageService
{
    Task<string> SaveEmpresaLogoAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<string> SaveTimeFotoAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task DeleteIfManagedAsync(string? publicUrl, CancellationToken cancellationToken);
}
