namespace Boleiroffice.Application.DTOs.Feed;

public sealed record ReagirFeedRequest(string Emoji);

public sealed record ReacaoContagem(string Emoji, int Total);

public sealed record ReacoesDesafioResponse(
    Guid DesafioId,
    string? MinhaReacao,
    IReadOnlyList<ReacaoContagem> Contagens);
