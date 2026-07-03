using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class ReacaoFeedService : IReacaoFeedService
{
    private static readonly HashSet<string> EmojisPermitidos = ["⚽", "🔥", "💪", "👏", "😮"];

    private readonly IReacaoFeedRepository _reacaoRepository;

    public ReacaoFeedService(IReacaoFeedRepository reacaoRepository)
    {
        _reacaoRepository = reacaoRepository;
    }

    public async Task<ReacoesDesafioResponse> GetByDesafioAsync(Guid desafioId, Guid? minhaEmpresaId, CancellationToken cancellationToken)
    {
        var reacoes = await _reacaoRepository.GetByDesafioAsync(desafioId, cancellationToken);
        return BuildResponse(desafioId, reacoes, minhaEmpresaId);
    }

    public async Task<ReacoesDesafioResponse> ReagirAsync(Guid desafioId, ReagirFeedRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        if (!EmojisPermitidos.Contains(request.Emoji))
            throw new BusinessException($"Emoji inválido. Permitidos: {string.Join(", ", EmojisPermitidos)}");

        var existente = await _reacaoRepository.GetByDesafioAndEmpresaAsync(desafioId, currentUser.EmpresaId, cancellationToken);

        if (existente is null)
        {
            var nova = new ReacaoFeed
            {
                Id = Guid.NewGuid(),
                DesafioId = desafioId,
                EmpresaId = currentUser.EmpresaId,
                Emoji = request.Emoji,
            };
            await _reacaoRepository.AddAsync(nova, cancellationToken);
        }
        else if (existente.Emoji == request.Emoji)
        {
            await _reacaoRepository.DeleteAsync(existente, cancellationToken);
        }
        else
        {
            existente.Emoji = request.Emoji;
            await _reacaoRepository.UpdateAsync(existente, cancellationToken);
        }

        var todas = await _reacaoRepository.GetByDesafioAsync(desafioId, cancellationToken);
        return BuildResponse(desafioId, todas, currentUser.EmpresaId);
    }

    private static ReacoesDesafioResponse BuildResponse(Guid desafioId, IReadOnlyList<ReacaoFeed> reacoes, Guid? minhaEmpresaId)
    {
        var minha = minhaEmpresaId.HasValue
            ? reacoes.FirstOrDefault(r => r.EmpresaId == minhaEmpresaId.Value)?.Emoji
            : null;

        var contagens = reacoes
            .GroupBy(r => r.Emoji)
            .Select(g => new ReacaoContagem(g.Key, g.Count()))
            .OrderByDescending(c => c.Total)
            .ToList();

        return new ReacoesDesafioResponse(desafioId, minha, contagens);
    }
}
