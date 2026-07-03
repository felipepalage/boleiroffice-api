using Boleiroffice.Application.Common.Security;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Security;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Infrastructure.Caching;
using Boleiroffice.Infrastructure.Persistence;
using Boleiroffice.Infrastructure.Repositories;
using Boleiroffice.Infrastructure.Security;
using Boleiroffice.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boleiroffice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var redisConnection = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
        }

        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ITimeRepository, TimeRepository>();
        services.AddScoped<IJogadorRepository, JogadorRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IDesafioRepository, DesafioRepository>();
        services.AddScoped<IConfirmacaoPresencaRepository, ConfirmacaoPresencaRepository>();
        services.AddScoped<IVotoMvpRepository, VotoMvpRepository>();
        services.AddScoped<IReacaoFeedRepository, ReacaoFeedRepository>();
        services.AddScoped<IDisponibilidadeTimeRepository, DisponibilidadeTimeRepository>();
        services.AddScoped<IMensagemDesafioRepository, MensagemDesafioRepository>();
        services.AddScoped<ITemporadaRepository, TemporadaRepository>();
        services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
        services.AddScoped<IComentarioFeedRepository, ComentarioFeedRepository>();
        services.AddScoped<IPostMuralRepository, PostMuralRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICacheService, DistributedCacheService>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();
        services.AddScoped<IQuadraRepository, QuadraRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<ITorneioRepository, TorneioRepository>();
        services.AddScoped<IAmistosoRepository, AmistosoRepository>();

        return services;
    }
}
