using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Jogadores;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class JogadorService : IJogadorService
{
    private readonly IJogadorRepository _jogadorRepository;
    private readonly IMapper _mapper;
    private readonly ITimeRepository _timeRepository;

    public JogadorService(IJogadorRepository jogadorRepository, ITimeRepository timeRepository, IMapper mapper)
    {
        _jogadorRepository = jogadorRepository;
        _timeRepository = timeRepository;
        _mapper = mapper;
    }

    public async Task<JogadorResponse> CreateAsync(JogadorCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(request.TimeId, "Time do jogador é obrigatório.");
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do jogador é obrigatório.");
        Guard.AgainstNullOrWhiteSpace(request.Posicao, "Posição do jogador é obrigatória.");

        var time = await _timeRepository.GetByIdAsync(request.TimeId, cancellationToken)
            ?? throw new NotFoundException("Time não encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Você só pode adicionar jogadores aos times da sua empresa.");
        }

        if (await _jogadorRepository.JerseyNumberExistsAsync(request.TimeId, request.NumeroCamisa, cancellationToken))
        {
            throw new BusinessException("Já existe um jogador com esse número de camisa nesse time.");
        }

        var jogador = new Jogador
        {
            Nome = request.Nome.Trim(),
            Posicao = request.Posicao.Trim(),
            NumeroCamisa = request.NumeroCamisa,
            TimeId = request.TimeId,
            Time = time
        };

        await _jogadorRepository.AddAsync(jogador, cancellationToken);

        return _mapper.Map<JogadorResponse>(jogador);
    }

    public async Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Jogador inválido.");

        var jogador = await _jogadorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        if (jogador.Time?.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Você só pode remover jogadores dos times da sua empresa.");
        }

        await _jogadorRepository.DeleteAsync(jogador, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JogadorResponse>> GetByTimeIdAsync(Guid timeId, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(timeId, "Time inválido.");

        var jogadores = await _jogadorRepository.GetByTimeIdAsync(timeId, cancellationToken);

        return jogadores.Select(_mapper.Map<JogadorResponse>).ToArray();
    }

    public async Task<JogadorPerfilResponse> GetPerfilAsync(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Jogador inválido.");

        var jogador = await _jogadorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Jogador não encontrado.");

        var time = jogador.Time;
        var (totalGols, jogosComGol, totalJogosTime) = await _jogadorRepository.GetGoalStatsAsync(
            jogador.TimeId, jogador.Nome, cancellationToken);

        return new JogadorPerfilResponse
        {
            Id = jogador.Id,
            Nome = jogador.Nome,
            Posicao = jogador.Posicao,
            NumeroCamisa = jogador.NumeroCamisa,
            TimeId = jogador.TimeId,
            TimeNome = time?.Nome ?? string.Empty,
            NomeEmpresa = time?.Empresa?.Nome ?? string.Empty,
            EscudoShape = time?.EscudoShape ?? 1,
            CorPrimaria = time?.CorPrimaria ?? "#DC2626",
            CorSecundaria = time?.CorSecundaria ?? "#111827",
            TotalGols = totalGols,
            JogosComGol = jogosComGol,
            TotalJogosTime = totalJogosTime,
        };
    }
}
