namespace Boleiroffice.Application.DTOs.Torneios;

public sealed record TorneioResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    int Formato,
    int Status,
    DateOnly DataInicio,
    DateOnly? DataFim,
    Guid EmpresaOrganizadoraId,
    string EmpresaOrganizadora,
    int TotalInscritos,
    IReadOnlyList<TorneioInscricaoResponse> Inscricoes,
    IReadOnlyList<PartidaTorneioResponse> Partidas,
    DateTime DataCriacao
);

public sealed record TorneioCreateRequest(
    string Nome,
    string? Descricao,
    int Formato,
    DateOnly DataInicio,
    DateOnly? DataFim
);

public sealed record TorneioInscricaoRequest(Guid TimeId);

public sealed record TorneioInscricaoResponse(
    Guid TimeId,
    string NomeTime,
    string NomeEmpresa,
    int EscudoShape,
    string CorPrimaria,
    string CorSecundaria,
    string? GrupoLetra,
    DateTime DataInscricao
);

public sealed record PartidaTorneioResponse(
    Guid Id,
    string Fase,
    int Status,
    Guid TimeMandanteId,
    string TimeMandante,
    int TimeMandanteEscudoShape,
    string TimeMandanteCorPrimaria,
    string TimeMandanteCorSecundaria,
    Guid? TimeVisitanteId,
    string? TimeVisitante,
    int TimeVisitanteEscudoShape,
    string TimeVisitanteCorPrimaria,
    string TimeVisitanteCorSecundaria,
    int? PlacarMandante,
    int? PlacarVisitante,
    DateOnly? DataJogo,
    TimeOnly? HoraJogo,
    string? Local
);

public sealed record AgendarPartidaRequest(
    Guid TimeMandanteId,
    Guid? TimeVisitanteId,
    string Fase,
    DateOnly? DataJogo,
    TimeOnly? HoraJogo,
    string? Local
);

public sealed record RegistrarResultadoPartidaRequest(int PlacarMandante, int PlacarVisitante);
