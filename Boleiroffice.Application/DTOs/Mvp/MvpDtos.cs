namespace Boleiroffice.Application.DTOs.Mvp;

public sealed record VotarMvpRequest(Guid JogadorVotadoId);

public sealed record MvpCandidatoResponse(
    Guid JogadorId,
    string Nome,
    string Posicao,
    int NumeroCamisa,
    string NomeTime,
    int TotalVotos);

public sealed record VotacaoMvpResponse(
    Guid DesafioId,
    Guid? JogadorVotadoIdPelaMinhaEmpresa,
    IReadOnlyList<MvpCandidatoResponse> Candidatos);
