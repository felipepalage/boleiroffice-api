using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace Boleiroffice.Infrastructure.Services;

public sealed class LocalImageStorageService : IImageStorageService
{
    private const string ManagedUploadsPrefix = "/uploads/";
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly IHostEnvironment _hostEnvironment;

    public LocalImageStorageService(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public Task<string> SaveEmpresaLogoAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        return SaveAsync(fileStream, fileName, "empresas", cancellationToken);
    }

    public Task<string> SaveTimeFotoAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        return SaveAsync(fileStream, fileName, "times", cancellationToken);
    }

    public Task DeleteIfManagedAsync(string? publicUrl, CancellationToken cancellationToken)
    {
        if (!TryGetManagedFilePath(publicUrl, out var filePath))
        {
            return Task.CompletedTask;
        }

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }

        return Task.CompletedTask;
    }

    private async Task<string> SaveAsync(Stream fileStream, string fileName, string category, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new BusinessException("Formato de imagem invalido. Use JPG, PNG ou WEBP.");
        }

        var rootPath = _hostEnvironment.ContentRootPath;
        var uploadsRoot = Path.Combine(rootPath, "wwwroot", "uploads", category);
        Directory.CreateDirectory(uploadsRoot);

        var safeFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(uploadsRoot, safeFileName);

        await using var output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(output, cancellationToken);

        return $"/uploads/{category}/{safeFileName}";
    }

    private bool TryGetManagedFilePath(string? publicUrl, out string filePath)
    {
        filePath = string.Empty;

        if (string.IsNullOrWhiteSpace(publicUrl) ||
            !publicUrl.StartsWith(ManagedUploadsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var relativePath = publicUrl[ManagedUploadsPrefix.Length..]
            .TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return false;
        }

        var uploadsRoot = Path.GetFullPath(Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "uploads"));
        var candidatePath = Path.GetFullPath(Path.Combine(uploadsRoot, relativePath));

        if (!candidatePath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        filePath = candidatePath;
        return true;
    }
}
