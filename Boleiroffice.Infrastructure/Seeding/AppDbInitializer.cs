using Boleiroffice.Domain.Entities;
using Boleiroffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boleiroffice.Infrastructure.Seeding;

public static class AppDbInitializer
{
    private const string ZitecCnpj = "54638076000176";
    private const string ZitecLogin = "admin@zitec.com.br";
    private const string ZitecPassword = "Zitec@2026!";

    public static async Task InitializeAsync(IServiceProvider serviceProvider, bool seedData, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        if (!seedData)
        {
            return;
        }

        await EnsureZitecAsync(context, cancellationToken);
    }

    private static async Task EnsureZitecAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var empresa = await context.Empresas.FirstOrDefaultAsync(x => x.Cnpj == ZitecCnpj, cancellationToken);
        if (empresa is null)
        {
            empresa = new Empresa
            {
                Nome = "Zitec",
                Cnpj = ZitecCnpj,
                Bairro = "Centro",
                Cidade = "Sao Paulo",
                DataCriacao = DateTime.UtcNow
            };

            await context.Empresas.AddAsync(empresa, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var time = await context.Times.FirstOrDefaultAsync(x => x.EmpresaId == empresa.Id && x.Nome == "Zitec", cancellationToken);
        if (time is null)
        {
            time = new Time
            {
                Nome = "Zitec",
                EmpresaId = empresa.Id,
                BairroBase = empresa.Bairro,
                Nivel = 3,
                DataCriacao = DateTime.UtcNow
            };

            await context.Times.AddAsync(time, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == ZitecLogin, cancellationToken);
        if (usuario is null)
        {
            usuario = new Usuario
            {
                Nome = "Admin Zitec",
                Email = ZitecLogin,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(ZitecPassword),
                EmpresaId = empresa.Id
            };

            await context.Usuarios.AddAsync(usuario, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
