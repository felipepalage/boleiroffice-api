using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Mvp;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class VotoMvpService : IVotoMvpService
{
    private readonly IVotoMvpRepository _votoRepository;
    private readonly IDesafioRepository _desafioRepository;
    private readonly IJogadorRepository _jogadorRepository;

    public VotoMvpService(
        IVotoMvpRepository votoRepository,
        IDesafioRepository desafioRepository,
        IJogadorRepository jogadorRepository)
    {
        _votoRepository = votoRepository;
        _desafioRepository = desafioRepository;
        _jogadorRepository = jogadorRepository;
    }

    public async Task<VotacaoMvpResponse> GetByDesafioAsync(Guid desafioId, Guid? minhaEmpresaId, CancellationToken cancellationToken)
    {
        var votos = await _votoRepository.GetByDesafioAsync(desafioId, cancellationToken);
        return BuildResponse(desafioId, votos, minhaEmpresaId);
    }

    public async Task<VotacaoMvpResponse> VotarAsync(Guid desafioId, VotarMvpRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        var desafio = await _desafioRepository.GetByIdAsync(desafioId, cancellationToken)
            ?? throw new NotFoundException("Desafio não encontrado.");

        if (desafio.Status != DesafioStatus.Finalizado)
            throw new BusinessException("Votação MVP só está disponível para partidas finalizadas.");

        var jogador = await _jogadorRepository.GetByIdAsync(request.JogadorVotadoId, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        var timePertenceAoDesafio =
            jogador.TimeId == desafio.TimeCriadorId ||
            jogador.TimeId == desafio.TimeDesafianteId;

        if (!timePertenceAoDesafio)
            throw new BusinessException("Esse jogador não participou deste desafio.");

        var votoExistente = await _votoRepository.GetByDesafioAndEmpresaAsync(desafioId, currentUser.EmpresaId, cancellationToken);

        if (votoExistente is null)
        {
            votoExistente = new VotoMvp
            {
                DesafioId = desafioId,
                JogadorVotadoId = request.JogadorVotadoId,
                VotantePorEmpresaId = currentUser.EmpresaId,
            };
        }
        else
        {
            votoExistente.JogadorVotadoId = request.JogadorVotadoId;
            votoExistente.DataVoto = DateTime.UtcNow;
        }

        await _votoRepository.UpsertAsync(votoExistente, cancellationToken);

        var todosVotos = await _votoRepository.GetByDesafioAsync(desafioId, cancellationToken);
        return BuildResponse(desafioId, todosVotos, currentUser.EmpresaId);
    }

    private static VotacaoMvpResponse BuildResponse(Guid desafioId, IReadOnlyList<VotoMvp> votos, Guid? minhaEmpresaId)
    {
        var meuVoto = minhaEmpresaId.HasValue
            ? votos.FirstOrDefault(v => v.VotantePorEmpresaId == minhaEmpresaId.Value)
            : null;

        var candidatos = votos
            .GroupBy(v => v.JogadorVotadoId)
            .Select(g =>
            {
                var jogador = g.First().JogadorVotado;
                return new MvpCandidatoResponse(
                    JogadorId: g.Key,
                    Nome: jogador?.Nome ?? "Desconhecido",
                    Posicao: jogador?.Posicao ?? "",
                    NumeroCamisa: jogador?.NumeroCamisa ?? 0,
                    NomeTime: jogador?.Time?.Nome ?? "",
                    TotalVotos: g.Count());
            })
            .OrderByDescending(c => c.TotalVotos)
            .ToList();

        return new VotacaoMvpResponse(
            DesafioId: desafioId,
            JogadorVotadoIdPelaMinhaEmpresa: meuVoto?.JogadorVotadoId,
            Candidatos: candidatos);
    }
}
