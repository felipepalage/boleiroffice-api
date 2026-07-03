using Boleiroffice.Application.DTOs.Temporada;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class TemporadaService : ITemporadaService
{
    private readonly ITemporadaRepository _repo;

    public TemporadaService(ITemporadaRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<TemporadaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await _repo.GetAllAsync(cancellationToken);
        return all.Select(ToResponse).ToList();
    }

    public async Task<TemporadaResponse> CreateAsync(TemporadaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DataFim <= request.DataInicio)
            throw new BusinessException("Data de fim deve ser posterior à data de início.");

        if (await _repo.ExistsByNomeAsync(request.Nome, cancellationToken))
            throw new BusinessException("Já existe uma temporada com esse nome.");

        var t = new Temporada
        {
            Nome = request.Nome,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
        };

        await _repo.AddAsync(t, cancellationToken);
        return ToResponse(t);
    }

    public async Task<TemporadaResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var temporada = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Temporada não encontrada.");

        var all = await _repo.GetAllAsync(cancellationToken);
        foreach (var t in all.Where(t => t.Ativa))
        {
            t.Ativa = false;
            await _repo.UpdateAsync(t, cancellationToken);
        }

        temporada.Ativa = true;
        await _repo.UpdateAsync(temporada, cancellationToken);
        return ToResponse(temporada);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var temporada = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Temporada não encontrada.");

        if (temporada.Ativa)
            throw new BusinessException("Não é possível excluir a temporada ativa.");

        await _repo.DeleteAsync(temporada, cancellationToken);
    }

    private static TemporadaResponse ToResponse(Temporada t) =>
        new(t.Id, t.Nome, t.DataInicio, t.DataFim, t.Ativa);
}
