using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Disponibilidade;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;
using Boleiroffice.Domain.Enums;

namespace Boleiroffice.Application.Services;

public sealed class DisponibilidadeService : IDisponibilidadeService
{
    private static readonly string[] DiaSemanaLabels =
        ["Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado"];

    private readonly IDisponibilidadeTimeRepository _repo;
    private readonly IDesafioRepository _desafioRepo;
    private readonly ITimeRepository _timeRepo;

    public DisponibilidadeService(
        IDisponibilidadeTimeRepository repo,
        IDesafioRepository desafioRepo,
        ITimeRepository timeRepo)
    {
        _repo = repo;
        _desafioRepo = desafioRepo;
        _timeRepo = timeRepo;
    }

    public async Task<IReadOnlyList<DisponibilidadeResponse>> GetByTimeAsync(Guid timeId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetByTimeAsync(timeId, cancellationToken);
        return items.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<BrowseDisponibilidadeItem>> BrowseAsync(
        string? cidade, string? bairro, int? diaSemana, Guid? meuTimeId, CancellationToken cancellationToken = default)
    {
        var all = await _repo.GetAllActiveAsync(cancellationToken);

        var query = all.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(x => x.Cidade.Contains(cidade, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(bairro))
            query = query.Where(x => x.Bairro.Contains(bairro, StringComparison.OrdinalIgnoreCase));

        if (diaSemana.HasValue)
            query = query.Where(x => x.DiaSemana == diaSemana.Value);

        if (meuTimeId.HasValue)
            query = query.Where(x => x.TimeId != meuTimeId.Value);

        var result = new List<BrowseDisponibilidadeItem>();

        foreach (var item in query)
        {
            var proximaData = ComputeProximaData(item.DiaSemana);
            var slotConflict = await _desafioRepo.TimeHasConflictAsync(item.TimeId, proximaData, null, cancellationToken);
            var myConflict = meuTimeId.HasValue
                && await _desafioRepo.TimeHasConflictAsync(meuTimeId.Value, proximaData, null, cancellationToken);

            result.Add(new BrowseDisponibilidadeItem(
                Id: item.Id,
                TimeId: item.TimeId,
                NomeTime: item.Time?.Nome ?? "Time",
                DiaSemana: item.DiaSemana,
                DiaSemanaLabel: DiaSemanaLabels[item.DiaSemana],
                Horario: item.Horario.ToString("HH:mm"),
                NomeLocal: item.NomeLocal,
                Bairro: item.Bairro,
                Cidade: item.Cidade,
                EnderecoCompleto: item.EnderecoCompleto,
                ProximaData: proximaData,
                Disponivel: !slotConflict && !myConflict));
        }

        return result.OrderByDescending(x => x.Disponivel).ThenBy(x => x.Cidade).ThenBy(x => x.Bairro).ToList();
    }

    public async Task<DisponibilidadeResponse> CreateAsync(
        CreateDisponibilidadeRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        if (request.DiaSemana < 0 || request.DiaSemana > 6)
            throw new BusinessException("DiaSemana deve ser entre 0 (Domingo) e 6 (Sábado).");

        if (!TimeOnly.TryParse(request.Horario, out var horario))
            throw new BusinessException("Horário inválido. Use o formato HH:mm.");

        var time = await _timeRepo.GetByIdAsync(request.TimeId, cancellationToken)
            ?? throw new NotFoundException("Time não encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Você não tem permissão para gerenciar esse time.");

        var conflict = await _repo.ExistsConflictAsync(request.TimeId, request.DiaSemana, horario, null, cancellationToken);
        if (conflict)
            throw new BusinessException("Já existe um slot para esse dia e horário.");

        var item = new DisponibilidadeTime
        {
            TimeId = request.TimeId,
            DiaSemana = request.DiaSemana,
            Horario = horario,
            NomeLocal = request.NomeLocal,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            EnderecoCompleto = request.EnderecoCompleto,
        };
        item.Time = time;

        await _repo.AddAsync(item, cancellationToken);
        return ToResponse(item);
    }

    public async Task<DisponibilidadeResponse> UpdateAsync(
        Guid id, Guid timeId, DisponibilidadeRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Slot não encontrado.");

        if (item.TimeId != timeId)
            throw new BusinessException("Você não tem permissão para editar esse slot.");

        if (!TimeOnly.TryParse(request.Horario, out var horario))
            throw new BusinessException("Horário inválido. Use o formato HH:mm.");

        var conflict = await _repo.ExistsConflictAsync(timeId, request.DiaSemana, horario, id, cancellationToken);
        if (conflict)
            throw new BusinessException("Já existe um slot para esse dia e horário.");

        item.DiaSemana = request.DiaSemana;
        item.Horario = horario;
        item.NomeLocal = request.NomeLocal;
        item.Bairro = request.Bairro;
        item.Cidade = request.Cidade;
        item.EnderecoCompleto = request.EnderecoCompleto;

        await _repo.UpdateAsync(item, cancellationToken);
        return ToResponse(item);
    }

    public async Task DeleteAsync(Guid id, Guid timeId, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Slot não encontrado.");

        if (item.TimeId != timeId)
            throw new BusinessException("Você não tem permissão para excluir esse slot.");

        await _repo.DeleteAsync(item, cancellationToken);
    }

    public async Task DesafiarDeSlotAsync(
        Guid slotId, DesafiarDeSlotBodyRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        var slot = await _repo.GetByIdAsync(slotId, cancellationToken)
            ?? throw new NotFoundException("Slot não encontrado.");

        var meuTime = await _timeRepo.GetByIdAsync(request.MeuTimeId, cancellationToken)
            ?? throw new NotFoundException("Seu time não foi encontrado.");

        if (meuTime.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Você não tem permissão para usar esse time.");

        if (await _desafioRepo.TimeHasConflictAsync(slot.TimeId, request.DataJogo, null, cancellationToken))
            throw new BusinessException("O time convidado já tem jogo nessa data.");

        if (await _desafioRepo.TimeHasConflictAsync(request.MeuTimeId, request.DataJogo, null, cancellationToken))
            throw new BusinessException("Seu time já tem jogo nessa data.");

        var desafio = new Desafio
        {
            TimeCriadorId = request.MeuTimeId,
            TimeDesafianteId = slot.TimeId,
            DataJogo = request.DataJogo,
            HoraJogo = slot.Horario,
            Local = slot.NomeLocal ?? slot.EnderecoCompleto ?? "A definir",
            Bairro = slot.Bairro,
            Nivel = meuTime.Nivel,
            Status = DesafioStatus.Aberto,
        };

        await _desafioRepo.AddAsync(desafio, cancellationToken);
    }

    private static DateOnly ComputeProximaData(int diaSemana)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var diff = ((diaSemana - (int)today.DayOfWeek) + 7) % 7;
        if (diff == 0) diff = 7;
        return today.AddDays(diff);
    }

    private static DisponibilidadeResponse ToResponse(DisponibilidadeTime item) => new(
        Id: item.Id,
        TimeId: item.TimeId,
        NomeTime: item.Time?.Nome ?? "Time",
        DiaSemana: item.DiaSemana,
        DiaSemanaLabel: DiaSemanaLabels[item.DiaSemana],
        Horario: item.Horario.ToString("HH:mm"),
        NomeLocal: item.NomeLocal,
        Bairro: item.Bairro,
        Cidade: item.Cidade,
        EnderecoCompleto: item.EnderecoCompleto,
        Ativo: item.Ativo);
}
