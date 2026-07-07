using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boleiroffice.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddFluentValidationAutoValidation();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IJogadorService, JogadorService>();
        services.AddScoped<IDesafioService, DesafioService>();
        services.AddScoped<IFeedService, FeedService>();
        services.AddScoped<IRankingService, RankingService>();
        services.AddScoped<IPresencaService, PresencaService>();
        services.AddScoped<IVotoMvpService, VotoMvpService>();
        services.AddScoped<IReacaoFeedService, ReacaoFeedService>();
        services.AddScoped<IDisponibilidadeService, DisponibilidadeService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<ICalendarioService, CalendarioService>();
        services.AddScoped<ITemporadaService, TemporadaService>();
        services.AddScoped<IFinanceiroService, FinanceiroService>();
        services.AddScoped<IComentarioFeedService, ComentarioFeedService>();
        services.AddScoped<IPostMuralService, PostMuralService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IRivalidadeService, RivalidadeService>();
        services.AddScoped<IConquistaService, ConquistaService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<IQuadraService, QuadraService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();
        services.AddScoped<ITorneioService, TorneioService>();
        services.AddScoped<IAmistosoService, AmistosoService>();
        services.AddScoped<IRachaoEventoService, RachaoEventoService>();
        services.AddScoped<IEstatisticasPublicasService, EstatisticasPublicasService>();
        services.AddScoped<IConviteMembroService, ConviteMembroService>();

        return services;
    }
}
