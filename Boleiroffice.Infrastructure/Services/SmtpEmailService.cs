using System.Net;
using System.Net.Mail;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Boleiroffice.Infrastructure.Services;

/// <summary>
/// Envio via SMTP configurado por variáveis de ambiente:
/// Smtp__Host, Smtp__Port (587), Smtp__User, Smtp__Password, Smtp__From, Smtp__EnableSsl (true).
/// Sem Host configurado, vira no-op — o app funciona normalmente sem e-mail.
/// </summary>
public sealed class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string para, string assunto, string htmlCorpo, CancellationToken cancellationToken)
    {
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(para))
        {
            return; // não configurado — no-op
        }

        try
        {
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Password"];
            var from = _config["Smtp:From"] ?? user ?? "no-reply@boleiroffice.com.br";
            var enableSsl = !bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) || ssl;

            using var message = new MailMessage(from, para, assunto, htmlCorpo) { IsBodyHtml = true };
            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrWhiteSpace(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
            };

            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Best-effort: apenas registra, nunca propaga.
            _logger.LogWarning(ex, "Falha ao enviar e-mail para {Para}", para);
        }
    }
}
