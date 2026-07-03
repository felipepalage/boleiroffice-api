namespace Boleiroffice.Application.DTOs.Quadras;

public sealed record QuadraResponse(
    Guid Id,
    string Nome,
    string Endereco,
    string Bairro,
    string Cidade,
    string? Estado,
    string? Cep,
    int? Capacidade,
    int TipoGrama,
    bool Iluminacao,
    bool Vestiario,
    string? FotoUrl,
    double NotaMedia,
    int TotalAvaliacoes,
    IReadOnlyList<AvaliacaoResponse> Avaliacoes,
    DateTime DataCriacao
);

public sealed record QuadraCreateRequest(
    string Nome,
    string Endereco,
    string Bairro,
    string Cidade,
    string? Estado,
    string? Cep,
    int? Capacidade,
    int TipoGrama,
    bool Iluminacao,
    bool Vestiario
);

public sealed record AvaliacaoCreateRequest(int Nota, string? Comentario);

public sealed record AvaliacaoResponse(
    Guid Id,
    string NomeEmpresa,
    int Nota,
    string? Comentario,
    DateTime DataCriacao
);
