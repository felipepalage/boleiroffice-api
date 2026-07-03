using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Api.Services;

public sealed class BackgroundFinalizationService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BackgroundFinalizationService> _logger;

    public BackgroundFinalizationService(IServiceProvider services, ILogger<BackgroundFinalizationService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            try
            {
                using var scope = _services.CreateScope();
                var desafioService = scope.ServiceProvider.GetRequiredService<IDesafioService>();
                await desafioService.AutoFinalizeExpiredAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao auto-finalizar resultados expirados.");
            }
        }
    }
}
