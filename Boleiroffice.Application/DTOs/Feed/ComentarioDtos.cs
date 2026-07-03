namespace Boleiroffice.Application.DTOs.Feed;

public record ComentarRequest(string Conteudo);

public record ComentarioResponse(
    Guid Id,
    Guid DesafioId,
    Guid EmpresaId,
    string NomeEmpresa,
    string Conteudo,
    DateTime DataComentario);
