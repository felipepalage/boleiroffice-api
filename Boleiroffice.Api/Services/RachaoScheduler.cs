using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Api.Services;

/// <summary>
/// Roda em segundo plano: sorteia os times dos eventos de rachão quando faltam ~2h
/// e remove eventos com mais de 3 dias. Verifica a cada 2 minutos.
/// </summary>
public sealed class RachaoScheduler : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(2);
    private readonly IServiceProvider _services;
    private readonly ILogger<RachaoScheduler> _logger;

    public RachaoScheduler(IServiceProvider services, ILogger<RachaoScheduler> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRachaoEventoService>();
                var sorteados = await service.SortearPendentesAsync(stoppingToken);
                var removidos = await service.LimparAntigosAsync(3, stoppingToken);
                if (sorteados > 0 || removidos > 0)
                    _logger.LogInformation("Rachão scheduler: {Sorteados} sorteado(s), {Removidos} antigo(s) removido(s).", sorteados, removidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no agendador do rachão.");
            }

            try
            {
                await Task.Delay(Intervalo, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
