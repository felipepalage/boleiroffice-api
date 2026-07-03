using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Torneios;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class TorneioService : ITorneioService
{
    private readonly ITorneioRepository _repo;
    private readonly ITimeRepository _timeRepo;

    public TorneioService(ITorneioRepository repo, ITimeRepository timeRepo)
    {
        _repo = repo;
        _timeRepo = timeRepo;
    }

    public async Task<TorneioResponse> CreateAsync(TorneioCreateRequest request, Guid empresaId, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do torneio é obrigatório.");

        var torneio = new Torneio
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            Formato = request.Formato,
            Status = 0, // Inscricoes
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            EmpresaOrganizadoraId = empresaId,
            DataCriacao = DateTime.UtcNow
        };

        await _repo.AddAsync(torneio, cancellationToken);
        return MapResponse(torneio);
    }

    public async Task<TorneioResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var t = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Torneio não encontrado.");
        return MapResponse(t);
    }

    public async Task<PagedResult<TorneioResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _repo.GetPagedAsync(pagination, cancellationToken);
        return result.Map(MapResponse);
    }

    public async Task<TorneioResponse> InscreverTimeAsync(Guid torneioId, TorneioInscricaoRequest request, Guid empresaId, CancellationToken cancellationToken)
    {
        var torneio = await _repo.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException("Torneio não encontrado.");

        if (torneio.Status != 0)
            throw new BusinessException("Inscrições encerradas para este torneio.");

        var time = await _timeRepo.GetByIdAsync(request.TimeId, cancellationToken)
            ?? throw new NotFoundException("Time não encontrado.");

        if (time.EmpresaId != empresaId)
            throw new BusinessException("Você só pode inscrever times da sua empresa.");

        if (torneio.Inscricoes.Any(i => i.TimeId == request.TimeId))
            throw new BusinessException("Time já está inscrito neste torneio.");

        var inscricao = new TorneioInscricao
        {
            Id = Guid.NewGuid(),
            TorneioId = torneioId,
            TimeId = request.TimeId,
            DataInscricao = DateTime.UtcNow
        };

        await _repo.AddInscricaoAsync(inscricao, cancellationToken);
        torneio = (await _repo.GetByIdAsync(torneioId, cancellationToken))!;
        return MapResponse(torneio);
    }

    public async Task<TorneioResponse> IniciarAsync(Guid id, Guid empresaId, CancellationToken cancellationToken)
    {
        var torneio = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Torneio não encontrado.");

        if (torneio.EmpresaOrganizadoraId != empresaId)
            throw new BusinessException("Apenas o organizador pode iniciar o torneio.");

        if (torneio.Status != 0)
            throw new BusinessException("Torneio não está mais em fase de inscrições.");

        if (torneio.Inscricoes.Count < 2)
            throw new BusinessException("São necessários pelo menos 2 times para iniciar.");

        torneio.Status = 1; // EmAndamento
        await _repo.UpdateAsync(torneio, cancellationToken);
        return MapResponse(torneio);
    }

    public async Task<PartidaTorneioResponse> AgendarPartidaAsync(Guid torneioId, AgendarPartidaRequest request, Guid empresaId, CancellationToken cancellationToken)
    {
        var torneio = await _repo.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException("Torneio não encontrado.");

        if (torneio.EmpresaOrganizadoraId != empresaId)
            throw new BusinessException("Apenas o organizador pode agendar partidas.");

        Guard.AgainstNullOrWhiteSpace(request.Fase, "Fase é obrigatória.");

        var partida = new PartidaTorneio
        {
            Id = Guid.NewGuid(),
            TorneioId = torneioId,
            TimeMandanteId = request.TimeMandanteId,
            TimeVisitanteId = request.TimeVisitanteId,
            Fase = request.Fase.Trim(),
            Status = 0,
            DataJogo = request.DataJogo,
            HoraJogo = request.HoraJogo,
            Local = request.Local?.Trim()
        };

        await _repo.AddPartidaAsync(partida, cancellationToken);
        var created = await _repo.GetPartidaByIdAsync(partida.Id, cancellationToken);
        return MapPartida(created!);
    }

    public async Task<PartidaTorneioResponse> RegistrarResultadoAsync(Guid torneioId, Guid partidaId, RegistrarResultadoPartidaRequest request, Guid empresaId, CancellationToken cancellationToken)
    {
        var torneio = await _repo.GetByIdAsync(torneioId, cancellationToken)
            ?? throw new NotFoundException("Torneio não encontrado.");

        if (torneio.EmpresaOrganizadoraId != empresaId)
            throw new BusinessException("Apenas o organizador pode registrar resultados.");

        var partida = await _repo.GetPartidaByIdAsync(partidaId, cancellationToken)
            ?? throw new NotFoundException("Partida não encontrada.");

        partida.PlacarMandante = request.PlacarMandante;
        partida.PlacarVisitante = request.PlacarVisitante;
        partida.Status = 1; // Realizada

        await _repo.UpdatePartidaAsync(partida, cancellationToken);
        return MapPartida(partida);
    }

    private static TorneioResponse MapResponse(Torneio t)
    {
        var inscricoes = t.Inscricoes.Select(i => new TorneioInscricaoResponse(
            i.TimeId,
            i.Time?.Nome ?? "—",
            i.Time?.Empresa?.Nome ?? "—",
            i.Time?.EscudoShape ?? 1,
            i.Time?.CorPrimaria ?? "#DC2626",
            i.Time?.CorSecundaria ?? "#111827",
            i.GrupoLetra,
            i.DataInscricao
        )).ToList();

        var partidas = t.Partidas.Select(MapPartida).ToList();

        return new TorneioResponse(
            t.Id, t.Nome, t.Descricao, t.Formato, t.Status, t.DataInicio, t.DataFim,
            t.EmpresaOrganizadoraId, t.EmpresaOrganizadora?.Nome ?? "—",
            inscricoes.Count, inscricoes, partidas, t.DataCriacao
        );
    }

    private static PartidaTorneioResponse MapPartida(PartidaTorneio p) => new(
        p.Id, p.Fase, p.Status,
        p.TimeMandanteId, p.TimeMandante?.Nome ?? "—",
        p.TimeMandante?.EscudoShape ?? 1, p.TimeMandante?.CorPrimaria ?? "#DC2626", p.TimeMandante?.CorSecundaria ?? "#111827",
        p.TimeVisitanteId, p.TimeVisitante?.Nome,
        p.TimeVisitante?.EscudoShape ?? 1, p.TimeVisitante?.CorPrimaria ?? "#DC2626", p.TimeVisitante?.CorSecundaria ?? "#111827",
        p.PlacarMandante, p.PlacarVisitante, p.DataJogo, p.HoraJogo, p.Local
    );
}
