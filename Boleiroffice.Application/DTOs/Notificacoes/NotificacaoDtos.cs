namespace Boleiroffice.Application.DTOs.Notificacoes;

public sealed record NotificacaoResponse(
    Guid Id,
    string Tipo,
    string Titulo,
    string Mensagem,
    string? Url,
    bool Lida,
    DateTime DataCriacao
);

public sealed record NotificacoesResumoResponse(
    int NaoLidas,
    IReadOnlyList<NotificacaoResponse> Itens
);
