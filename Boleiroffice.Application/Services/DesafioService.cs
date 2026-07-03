using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Desafios;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class DesafioService : IDesafioService
{
    private readonly IDesafioRepository _desafioRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notifications;
    private readonly ITimeRepository _timeRepository;

    public DesafioService(IDesafioRepository desafioRepository, ITimeRepository timeRepository, IMapper mapper, INotificationService notifications)
    {
        _desafioRepository = desafioRepository;
        _timeRepository = timeRepository;
        _mapper = mapper;
        _notifications = notifications;
    }

    public async Task<DesafioResponse> CreateAsync(DesafioCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(request.TimeCriadorId, "Time criador e obrigatorio.");
        Guard.AgainstDefault(request.TimeConvidadoId, "Time convidado e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Local, "Local do jogo e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Bairro, "Bairro do jogo e obrigatorio.");
        Guard.AgainstInvalidLevel(request.Nivel, "Nivel do desafio deve estar entre 1 e 5.");

        var timeCriador = await _timeRepository.GetByIdAsync(request.TimeCriadorId, cancellationToken)
            ?? throw new NotFoundException("Time criador nao encontrado.");

        var timeConvidado = await _timeRepository.GetByIdAsync(request.TimeConvidadoId, cancellationToken)
            ?? throw new NotFoundException("Time convidado nao encontrado.");

        if (timeCriador.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Voce so pode criar desafios pelos times da sua empresa.");
        }

        if (timeConvidado.EmpresaId == currentUser.EmpresaId)
        {
            throw new BusinessException("O convite precisa ser enviado para outra empresa.");
        }

        if (timeCriador.BloqueioAte.HasValue && timeCriador.BloqueioAte.Value > DateTime.UtcNow)
        {
            throw new BusinessException($"Seu time esta suspenso ate {timeCriador.BloqueioAte.Value:dd/MM/yyyy} por acumular 3 penalidades.");
        }

        if (timeConvidado.BloqueioAte.HasValue && timeConvidado.BloqueioAte.Value > DateTime.UtcNow)
        {
            throw new BusinessException("O time convidado esta suspenso e nao pode receber novos desafios no momento.");
        }

        if (await _desafioRepository.TimeHasConflictAsync(request.TimeCriadorId, request.DataJogo, null, cancellationToken))
        {
            throw new BusinessException("Esse time ja possui desafio marcado para essa data.");
        }

        if (await _desafioRepository.TimeHasConflictAsync(request.TimeConvidadoId, request.DataJogo, null, cancellationToken))
        {
            throw new BusinessException("O time convidado ja possui desafio marcado para essa data.");
        }

        var desafio = new Desafio
        {
            TimeCriadorId = request.TimeCriadorId,
            TimeCriador = timeCriador,
            TimeDesafianteId = request.TimeConvidadoId,
            TimeDesafiante = timeConvidado,
            DataJogo = request.DataJogo,
            HoraJogo = request.HoraJogo,
            Local = request.Local.Trim(),
            Bairro = request.Bairro.Trim(),
            Nivel = request.Nivel,
            Status = DesafioStatus.Aberto,
            DataCriacao = DateTime.UtcNow
        };

        await _desafioRepository.AddAsync(desafio, cancellationToken);

        if (timeConvidado.EmpresaId != Guid.Empty)
        {
            await _notifications.SendToEmpresaAsync(timeConvidado.EmpresaId, new AppNotification(
                Tipo: "DesafioCriado",
                Titulo: "Novo desafio recebido!",
                Mensagem: $"{timeCriador.Nome} convidou {timeConvidado.Nome} para um amistoso em {desafio.DataJogo:dd/MM}.",
                Url: "/"), cancellationToken);
        }

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<PagedResult<DesafioResponse>> GetOpenAsync(string? bairro, DateOnly? dataJogo, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var desafios = await _desafioRepository.GetOpenAsync(bairro, dataJogo, pagination.Normalize(), cancellationToken);
        return desafios.Map(_mapper.Map<DesafioResponse>);
    }

    public async Task<DesafioResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<DesafioResponse> AcceptAsync(Guid id, AcceptDesafioRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");
        Guard.AgainstDefault(request.TimeDesafianteId, "Time desafiante e obrigatorio.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        if (desafio.Status != DesafioStatus.Aberto)
        {
            throw new BusinessException("Apenas convites pendentes podem ser aceitos.");
        }

        if (!desafio.TimeDesafianteId.HasValue || desafio.TimeDesafianteId.Value != request.TimeDesafianteId)
        {
            throw new BusinessException("Esse convite pertence a outro time.");
        }

        var timeDesafiante = await _timeRepository.GetByIdAsync(request.TimeDesafianteId, cancellationToken)
            ?? throw new NotFoundException("Time desafiante nao encontrado.");

        if (timeDesafiante.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Voce so pode aceitar convites com times da sua empresa.");
        }

        if (timeDesafiante.Id == desafio.TimeCriadorId)
        {
            throw new BusinessException("Um time nao pode desafiar a si proprio.");
        }

        if (desafio.TimeCriador is not null && desafio.TimeCriador.EmpresaId == timeDesafiante.EmpresaId)
        {
            throw new BusinessException("O desafio deve ser aceito por um time de outra empresa.");
        }

        if (await _desafioRepository.TimeHasConflictAsync(request.TimeDesafianteId, desafio.DataJogo, desafio.Id, cancellationToken))
        {
            throw new BusinessException("Esse time ja possui desafio marcado para essa data.");
        }

        desafio.TimeDesafiante = timeDesafiante;
        desafio.Aceitar(request.TimeDesafianteId);

        await _desafioRepository.UpdateAsync(desafio, cancellationToken);

        if (desafio.TimeCriador?.EmpresaId is Guid criadorEmpresaId && criadorEmpresaId != Guid.Empty)
        {
            await _notifications.SendToEmpresaAsync(criadorEmpresaId, new AppNotification(
                Tipo: "DesafioAceito",
                Titulo: "Desafio aceito!",
                Mensagem: $"{timeDesafiante.Nome} aceitou o amistoso de {desafio.DataJogo:dd/MM}.",
                Url: "/"), cancellationToken);
        }

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<DesafioResponse> CancelAsync(Guid id, CancelDesafioRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        if (desafio.Status == DesafioStatus.Finalizado)
        {
            throw new BusinessException("Nao e possivel cancelar um desafio finalizado.");
        }

        GetAuthorizedTeamId(desafio, currentUser);
        desafio.Cancelar();
        await _desafioRepository.UpdateAsync(desafio, cancellationToken);

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<DesafioResponse> RegisterResultAsync(Guid id, RegisterResultRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        if (desafio.Status != DesafioStatus.Aceito && desafio.Status != DesafioStatus.ResultadoPendente)
        {
            throw new BusinessException("Somente desafios aceitos ou aguardando confirmacao podem receber resultado.");
        }

        var timeAtualId = GetAuthorizedTeamId(desafio, currentUser);

        if (desafio.Status == DesafioStatus.ResultadoPendente &&
            desafio.PlacarCriadorProposto == request.PlacarCriador &&
            desafio.PlacarDesafianteProposto == request.PlacarDesafiante)
        {
            desafio.ConfirmarResultado(timeAtualId);
        }
        else
        {
            desafio.ProporResultado(timeAtualId, request.PlacarCriador, request.PlacarDesafiante);
            var empresaOposta = timeAtualId == desafio.TimeCriadorId
                ? desafio.TimeDesafiante?.EmpresaId
                : desafio.TimeCriador?.EmpresaId;

            if (empresaOposta.HasValue && empresaOposta.Value != Guid.Empty)
            {
                await _notifications.SendToEmpresaAsync(empresaOposta.Value, new AppNotification(
                    Tipo: "ResultadoPendente",
                    Titulo: "Resultado aguardando confirmação",
                    Mensagem: $"Placar proposto: {request.PlacarCriador} x {request.PlacarDesafiante}. Confirme ou conteste.",
                    Url: "/"), cancellationToken);
            }
        }

        await _desafioRepository.UpdateAsync(desafio, cancellationToken);

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<DesafioResponse> ConfirmResultAsync(Guid id, ConfirmResultRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        if (desafio.Status != DesafioStatus.ResultadoPendente)
        {
            throw new BusinessException("Nao existe resultado pendente para confirmacao.");
        }

        if (desafio.PlacarCriadorProposto != request.PlacarCriador || desafio.PlacarDesafianteProposto != request.PlacarDesafiante)
        {
            throw new BusinessException("O placar informado nao corresponde ao resultado pendente.");
        }

        var timeAtualId = GetAuthorizedTeamId(desafio, currentUser);
        desafio.ConfirmarResultado(timeAtualId);

        await _desafioRepository.UpdateAsync(desafio, cancellationToken);

        return _mapper.Map<DesafioResponse>(desafio);
    }

    public async Task<DesafioResponse> RegisterScorersAsync(Guid id, RegistrarArtilheirosRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Desafio invalido.");

        var desafio = await _desafioRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado.");

        if (desafio.Status != DesafioStatus.Finalizado)
        {
            throw new BusinessException("Os artilheiros so podem ser registrados depois do placar final confirmado.");
        }

        GetAuthorizedTeamId(desafio, currentUser);

        var golsCriador = NormalizeScorers(request.GolsCriador, desafio.TimeCriadorId);
        var golsDesafiante = NormalizeScorers(request.GolsDesafiante, desafio.TimeDesafianteId);

        if (golsCriador.Sum(x => x.QuantidadeGols) != (desafio.PlacarCriador ?? 0))
        {
            throw new BusinessException("A soma dos gols do time criador precisa bater com o placar final.");
        }

        if (golsDesafiante.Sum(x => x.QuantidadeGols) != (desafio.PlacarDesafiante ?? 0))
        {
            throw new BusinessException("A soma dos gols do time desafiante precisa bater com o placar final.");
        }

        var gols = golsCriador.Concat(golsDesafiante)
            .Select(item => new GolPartida
            {
                DesafioId = desafio.Id,
                TimeId = item.TimeId,
                NomeAutor = item.NomeAutor,
                QuantidadeGols = item.QuantidadeGols,
                DataCriacao = DateTime.UtcNow
            })
            .ToArray();

        await _desafioRepository.ReplaceScorersAsync(desafio.Id, gols, cancellationToken);

        var atualizado = await _desafioRepository.GetByIdAsync(desafio.Id, cancellationToken)
            ?? throw new NotFoundException("Desafio nao encontrado apos registrar artilheiros.");

        return _mapper.Map<DesafioResponse>(atualizado);
    }

    public async Task<PagedResult<DesafioResponse>> GetByTimeIdAsync(Guid timeId, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(timeId, "Time invalido.");

        var desafios = await _desafioRepository.GetByTimeIdAsync(timeId, pagination.Normalize(), cancellationToken);
        return desafios.Map(_mapper.Map<DesafioResponse>);
    }

    public async Task<PagedResult<SuggestedChallengeResponse>> GetSuggestedAsync(Guid timeId, DateOnly dataJogo, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(timeId, "Time invalido.");

        var time = await _timeRepository.GetByIdAsync(timeId, cancellationToken)
            ?? throw new NotFoundException("Time nao encontrado.");

        if (await _desafioRepository.TimeHasConflictAsync(time.Id, dataJogo, null, cancellationToken))
        {
            throw new BusinessException("Esse time ja possui desafio marcado para a data informada.");
        }

        return await _desafioRepository.GetSuggestedAsync(timeId, dataJogo, pagination.Normalize(), cancellationToken);
    }

    public async Task AutoFinalizeExpiredAsync(CancellationToken cancellationToken)
    {
        var expiredBefore = DateTime.UtcNow.AddHours(-24);
        var pendentes = await _desafioRepository.GetExpiredPendingResultsAsync(expiredBefore, cancellationToken);
        foreach (var desafio in pendentes)
        {
            var naoAceitanteId = desafio.ResultadoPropostoPorTimeId == desafio.TimeCriadorId
                ? desafio.TimeDesafianteId
                : desafio.TimeCriadorId;

            desafio.FinalizarPorTimeout();
            await _desafioRepository.UpdateAsync(desafio, cancellationToken);

            if (naoAceitanteId.HasValue)
            {
                var time = await _timeRepository.GetByIdAsync(naoAceitanteId.Value, cancellationToken);
                if (time is not null)
                {
                    time.PenalidadesCount++;
                    if (time.PenalidadesCount >= 3)
                    {
                        time.BloqueioAte = DateTime.UtcNow.AddDays(7);
                        time.PenalidadesCount = 0;

                        if (time.Empresa?.Id is Guid empId && empId != Guid.Empty)
                        {
                            await _notifications.SendToEmpresaAsync(empId, new AppNotification(
                                Tipo: "TimeSuspenso",
                                Titulo: "Time suspenso por 7 dias",
                                Mensagem: $"{time.Nome} acumulou 3 penalidades e esta suspenso ate {time.BloqueioAte.Value:dd/MM/yyyy}.",
                                Url: "/"), cancellationToken);
                        }
                    }

                    await _timeRepository.UpdateAsync(time, cancellationToken);
                }
            }
        }
    }

    private static Guid GetAuthorizedTeamId(Desafio desafio, CurrentUser currentUser)
    {
        var timeAtualId = desafio.TimeCriador?.EmpresaId == currentUser.EmpresaId
            ? desafio.TimeCriadorId
            : desafio.TimeDesafiante?.EmpresaId == currentUser.EmpresaId
                ? desafio.TimeDesafianteId
                : null;

        if (!timeAtualId.HasValue)
        {
            throw new BusinessException("Voce nao pode agir sobre esse desafio.");
        }

        return timeAtualId.Value;
    }

    private static IReadOnlyCollection<(Guid TimeId, string NomeAutor, int QuantidadeGols)> NormalizeScorers(IEnumerable<GolPartidaRequest> gols, Guid? timeId)
    {
        if (!timeId.HasValue)
        {
            return Array.Empty<(Guid, string, int)>();
        }

        return gols
            .Where(x => !string.IsNullOrWhiteSpace(x.NomeAutor) && x.QuantidadeGols > 0)
            .Select(x => (timeId.Value, x.NomeAutor.Trim(), x.QuantidadeGols))
            .ToArray();
    }
}
