namespace Boleiroffice.Application.DTOs.Mural;

public record PostMuralRequest(string Titulo, string Conteudo);

public record PostMuralResponse(
    Guid Id,
    Guid EmpresaId,
    string NomeEmpresa,
    string Titulo,
    string Conteudo,
    string NomeAutor,
    DateTime DataPublicacao);
