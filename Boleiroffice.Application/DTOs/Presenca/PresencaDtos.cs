namespace Boleiroffice.Application.DTOs.Presenca;

public sealed class ConfirmarPresencaRequest
{
    public Guid JogadorId { get; init; }
    public bool Confirmado { get; init; }
}

public sealed class PresencaResponse
{
    public Guid Id { get; init; }
    public Guid JogadorId { get; init; }
    public string NomeJogador { get; init; } = string.Empty;
    public string Posicao { get; init; } = string.Empty;
    public int NumeroCamisa { get; init; }
    public bool Confirmado { get; init; }
}

public sealed class PresencaDesafioResponse
{
    public Guid DesafioId { get; init; }
    public IReadOnlyCollection<PresencaResponse> Confirmados { get; init; } = Array.Empty<PresencaResponse>();
    public IReadOnlyCollection<PresencaResponse> Pendentes { get; init; } = Array.Empty<PresencaResponse>();
}
