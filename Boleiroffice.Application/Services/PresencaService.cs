using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Presenca;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class PresencaService : IPresencaService
{
    private readonly IConfirmacaoPresencaRepository _presencaRepository;
    private readonly IDesafioRepository _desafioRepository;
    private readonly IJogadorRepository _jogadorRepository;

    public PresencaService(
        IConfirmacaoPresencaRepository presencaRepository,
        IDesafioRepository desafioRepository,
        IJogadorRepository jogadorRepository)
    {
        _presencaRepository = presencaRepository;
        _desafioRepository = desafioRepository;
        _jogadorRepository = jogadorRepository;
    }

    public async Task<PresencaResponse> ConfirmarAsync(Guid desafioId, ConfirmarPresencaRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(desafioId, "Desafio inválido.");
        Guard.AgainstDefault(request.JogadorId, "Jogador inválido.");

        var desafio = await _desafioRepository.GetByIdAsync(desafioId, cancellationToken)
            ?? throw new NotFoundException("Desafio não encontrado.");

        if (desafio.Status != DesafioStatus.Aceito && desafio.Status != DesafioStatus.ResultadoPendente)
        {
            throw new BusinessException("Confirmações só são permitidas para desafios aceitos.");
        }

        var jogador = await _jogadorRepository.GetByIdAsync(request.JogadorId, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        if (jogador.Time?.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Você só pode confirmar presença de jogadores da sua empresa.");
        }

        var timePertenceAoDesafio =
            jogador.TimeId == desafio.TimeCriadorId ||
            jogador.TimeId == desafio.TimeDesafianteId;

        if (!timePertenceAoDesafio)
        {
            throw new BusinessException("Esse jogador não participa deste desafio.");
        }

        var confirmacao = await _presencaRepository.GetAsync(request.JogadorId, desafioId, cancellationToken);

        if (confirmacao is null)
        {
            confirmacao = new ConfirmacaoPresenca
            {
                JogadorId = request.JogadorId,
                DesafioId = desafioId,
                Confirmado = request.Confirmado,
                Jogador = jogador,
            };
        }
        else
        {
            confirmacao.Confirmado = request.Confirmado;
        }

        await _presencaRepository.UpsertAsync(confirmacao, cancellationToken);

        return ToResponse(confirmacao, jogador);
    }

    public async Task<PresencaDesafioResponse> GetByDesafioAsync(Guid desafioId, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(desafioId, "Desafio inválido.");

        var confirmacoes = await _presencaRepository.GetByDesafioIdAsync(desafioId, cancellationToken);

        return new PresencaDesafioResponse
        {
            DesafioId = desafioId,
            Confirmados = confirmacoes
                .Where(c => c.Confirmado && c.Jogador is not null)
                .Select(c => ToResponse(c, c.Jogador!))
                .ToArray(),
            Pendentes = confirmacoes
                .Where(c => !c.Confirmado && c.Jogador is not null)
                .Select(c => ToResponse(c, c.Jogador!))
                .ToArray(),
        };
    }

    private static PresencaResponse ToResponse(ConfirmacaoPresenca c, Domain.Entities.Jogador j) => new()
    {
        Id = c.Id,
        JogadorId = j.Id,
        NomeJogador = j.Nome,
        Posicao = j.Posicao,
        NumeroCamisa = j.NumeroCamisa,
        Confirmado = c.Confirmado,
    };
}
