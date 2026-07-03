namespace Boleiroffice.Application.DTOs.Amistoso;

// ---------- Elenco (aba Geral / Pagamentos) ----------

public sealed record JogadorAmistosoResponse(
    Guid Id,
    string Nome,
    bool Ativo,
    bool PagouMensalidade,
    DateTime DataCriacao
);

public sealed record JogadorAmistosoCreateRequest(string Nome);

public sealed record PagamentoUpdateRequest(bool Pagou);

// ---------- Sorteio / Times (aba Times) ----------

public sealed record SortearRequest(IReadOnlyList<Guid> JogadorIds, int NumeroTimes);

public sealed record TimeAmistosoResponse(
    Guid Id,
    string Nome,
    int Ordem,
    DateTime DataSorteio,
    IReadOnlyList<string> Jogadores
);

// ---------- Partidas (aba Rachão) ----------

public sealed record IniciarPartidaRequest(string? Time1Nome, string? Time2Nome);

public sealed record RegistrarGolRequest(int TimeNumero, string AutorNome, string? AssistenteNome);

public sealed record FinalizarPartidaRequest(int DuracaoSegundos);

public sealed record GolAmistosoResponse(
    Guid Id,
    int TimeNumero,
    string AutorNome,
    string? AssistenteNome,
    DateTime DataCriacao
);

public sealed record PartidaAmistosoResponse(
    Guid Id,
    string Time1Nome,
    string Time2Nome,
    int Time1Gols,
    int Time2Gols,
    DateTime DataInicio,
    DateTime? DataFim,
    int DuracaoSegundos,
    bool Finalizada,
    IReadOnlyList<GolAmistosoResponse> Gols
);

// ---------- Ranking (artilheiros / garçons) ----------

public sealed record ArtilheiroAmistosoResponse(string Nome, int Total);

// ---------- Resumo do dia (flyer) ----------

public sealed record ResumoDiaResponse(
    DateTime Data,
    int TotalPartidas,
    int TotalGols,
    ArtilheiroAmistosoResponse? ArtilheiroDia,
    ArtilheiroAmistosoResponse? GarcomDia
);
