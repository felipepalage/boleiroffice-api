namespace Boleiroffice.Application.Interfaces.Services;

/// <summary>Envio de e-mail transacional. Best-effort: nunca deve derrubar o fluxo chamador.</summary>
public interface IEmailService
{
    Task SendAsync(string para, string assunto, string htmlCorpo, CancellationToken cancellationToken);
}
