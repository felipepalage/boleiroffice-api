namespace Boleiroffice.Application.DTOs.Chat;

public record EnviarMensagemRequest(string Conteudo);

public record MensagemResponse(
    Guid Id,
    Guid DesafioId,
    Guid EmpresaId,
    string NomeEmpresa,
    string Conteudo,
    DateTime DataEnvio);
