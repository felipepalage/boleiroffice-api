namespace Boleiroffice.Application.DTOs.Disponibilidade;

public record DisponibilidadeRequest(
    int DiaSemana,
    string Horario,
    string? NomeLocal,
    string Bairro,
    string Cidade,
    string? EnderecoCompleto);

public record CreateDisponibilidadeRequest(
    Guid TimeId,
    int DiaSemana,
    string Horario,
    string? NomeLocal,
    string Bairro,
    string Cidade,
    string? EnderecoCompleto);

public record DisponibilidadeResponse(
    Guid Id,
    Guid TimeId,
    string NomeTime,
    int DiaSemana,
    string DiaSemanaLabel,
    string Horario,
    string? NomeLocal,
    string Bairro,
    string Cidade,
    string? EnderecoCompleto,
    bool Ativo);

public record BrowseDisponibilidadeItem(
    Guid Id,
    Guid TimeId,
    string NomeTime,
    int DiaSemana,
    string DiaSemanaLabel,
    string Horario,
    string? NomeLocal,
    string Bairro,
    string Cidade,
    string? EnderecoCompleto,
    DateOnly ProximaData,
    bool Disponivel);

public record DesafiarDeSlotBodyRequest(Guid MeuTimeId, DateOnly DataJogo);
