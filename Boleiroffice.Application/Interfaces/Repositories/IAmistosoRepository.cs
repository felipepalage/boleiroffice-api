using Boleiroffice.Application.Common.Models;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Interfaces.Repositories;

public interface IAmistosoRepository
{
    // Elenco
    Task<IReadOnlyList<JogadorAmistoso>> GetJogadoresAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<JogadorAmistoso?> GetJogadorByIdAsync(Guid id, Guid empresaId, CancellationToken cancellationToken);
    Task AddJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken);
    Task UpdateJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken);
    Task RemoveJogadorAsync(JogadorAmistoso jogador, CancellationToken cancellationToken);
    Task ZerarPagamentosAsync(Guid empresaId, CancellationToken cancellationToken);

    // Sorteio / Times
    Task<IReadOnlyList<TimeAmistoso>> GetTimesAsync(Guid empresaId, CancellationToken cancellationToken);
    Task ReplaceTimesAsync(Guid empresaId, IReadOnlyList<TimeAmistoso> novosTimes, CancellationToken cancellationToken);

    // Partidas
    Task AddPartidaAsync(PartidaAmistoso partida, CancellationToken cancellationToken);
    Task<PartidaAmistoso?> GetPartidaByIdAsync(Guid id, Guid empresaId, CancellationToken cancellationToken);
    Task UpdatePartidaAsync(PartidaAmistoso partida, CancellationToken cancellationToken);
    Task<PagedResult<PartidaAmistoso>> GetPartidasPagedAsync(Guid empresaId, PaginationParameters pagination, CancellationToken cancellationToken);

    // Gols / ranking / resumo
    Task<IReadOnlyList<GolAmistoso>> GetGolsByEmpresaAsync(Guid empresaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PartidaAmistoso>> GetPartidasFinalizadasNoDiaAsync(Guid empresaId, DateOnly dia, CancellationToken cancellationToken);
}
