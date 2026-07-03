namespace Boleiroffice.Application.DTOs.Temporada;

public record TemporadaRequest(string Nome, DateOnly DataInicio, DateOnly DataFim);
public record TemporadaResponse(Guid Id, string Nome, DateOnly DataInicio, DateOnly DataFim, bool Ativa);
