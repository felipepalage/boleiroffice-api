using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Chat;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class ChatService : IChatService
{
    private readonly IMensagemDesafioRepository _repo;
    private readonly IDesafioRepository _desafioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly ITimeRepository _timeRepo;

    public ChatService(
        IMensagemDesafioRepository repo,
        IDesafioRepository desafioRepo,
        IEmpresaRepository empresaRepo,
        ITimeRepository timeRepo)
    {
        _repo = repo;
        _desafioRepo = desafioRepo;
        _empresaRepo = empresaRepo;
        _timeRepo = timeRepo;
    }

    public async Task<IReadOnlyList<MensagemResponse>> GetByDesafioAsync(
        Guid desafioId, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        await ValidateAccessAsync(desafioId, currentUser, cancellationToken);
        var msgs = await _repo.GetByDesafioAsync(desafioId, cancellationToken);
        return msgs.Select(ToResponse).ToList();
    }

    public async Task<MensagemResponse> SendAsync(
        Guid desafioId, EnviarMensagemRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Conteudo) || request.Conteudo.Length > 1000)
            throw new BusinessException("Mensagem deve ter entre 1 e 1000 caracteres.");

        await ValidateAccessAsync(desafioId, currentUser, cancellationToken);

        var empresa = await _empresaRepo.GetByIdAsync(currentUser.EmpresaId, cancellationToken);
        var nomeEmpresa = empresa?.Nome ?? currentUser.Nome;

        var msg = new MensagemDesafio
        {
            DesafioId = desafioId,
            EmpresaId = currentUser.EmpresaId,
            NomeEmpresa = nomeEmpresa,
            Conteudo = request.Conteudo.Trim(),
        };

        await _repo.AddAsync(msg, cancellationToken);
        return ToResponse(msg);
    }

    private async Task ValidateAccessAsync(Guid desafioId, CurrentUser currentUser, CancellationToken ct)
    {
        var desafio = await _desafioRepo.GetByIdAsync(desafioId, ct)
            ?? throw new NotFoundException("Desafio não encontrado.");

        var timeCriador = await _timeRepo.GetByIdAsync(desafio.TimeCriadorId, ct);
        var timeDesafiante = desafio.TimeDesafianteId.HasValue
            ? await _timeRepo.GetByIdAsync(desafio.TimeDesafianteId.Value, ct)
            : null;

        var pertence = timeCriador?.EmpresaId == currentUser.EmpresaId
            || timeDesafiante?.EmpresaId == currentUser.EmpresaId;

        if (!pertence)
            throw new BusinessException("Você não tem acesso ao chat deste desafio.");
    }

    private static MensagemResponse ToResponse(MensagemDesafio m) => new(
        m.Id, m.DesafioId, m.EmpresaId, m.NomeEmpresa, m.Conteudo, m.DataEnvio);
}
