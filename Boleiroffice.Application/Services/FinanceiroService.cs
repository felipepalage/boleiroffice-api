using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Financeiro;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class FinanceiroService : IFinanceiroService
{
    private readonly IFinanceiroRepository _repo;
    private readonly ITimeRepository _timeRepo;

    public FinanceiroService(IFinanceiroRepository repo, ITimeRepository timeRepo)
    {
        _repo = repo;
        _timeRepo = timeRepo;
    }

    public async Task<FinanceiroSummaryResponse> GetByTimeAsync(
        Guid timeId, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        await ValidateOwnershipAsync(timeId, currentUser, cancellationToken);

        var itens = await _repo.GetByTimeAsync(timeId, cancellationToken);
        var receitas = itens.Where(i => i.Tipo == "Receita").Sum(i => i.Valor);
        var despesas = itens.Where(i => i.Tipo == "Despesa").Sum(i => i.Valor);

        return new FinanceiroSummaryResponse(
            TotalReceitas: receitas,
            TotalDespesas: despesas,
            Saldo: receitas - despesas,
            Itens: itens.Select(ToResponse).ToList());
    }

    public async Task<FinanceiroItemResponse> CreateAsync(
        Guid timeId, FinanceiroItemRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        await ValidateOwnershipAsync(timeId, currentUser, cancellationToken);

        var item = new FinanceiroItem
        {
            TimeId = timeId,
            Descricao = request.Descricao,
            Valor = request.Valor,
            Tipo = request.Tipo,
            Categoria = request.Categoria,
            DataVencimento = request.DataVencimento,
        };

        await _repo.AddAsync(item, cancellationToken);
        return ToResponse(item);
    }

    public async Task<FinanceiroItemResponse> MarcarPagoAsync(
        Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Item financeiro não encontrado.");

        await ValidateOwnershipAsync(item.TimeId, currentUser, cancellationToken);

        item.Pago = true;
        await _repo.UpdateAsync(item, cancellationToken);
        return ToResponse(item);
    }

    public async Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Item financeiro não encontrado.");

        await ValidateOwnershipAsync(item.TimeId, currentUser, cancellationToken);
        await _repo.DeleteAsync(item, cancellationToken);
    }

    private async Task ValidateOwnershipAsync(Guid timeId, CurrentUser currentUser, CancellationToken ct)
    {
        var time = await _timeRepo.GetByIdAsync(timeId, ct)
            ?? throw new NotFoundException("Time não encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Você não tem permissão para acessar as finanças desse time.");
    }

    private static FinanceiroItemResponse ToResponse(FinanceiroItem i) => new(
        i.Id, i.TimeId, i.Descricao, i.Valor, i.Tipo, i.Categoria, i.DataVencimento, i.Pago, i.DataCriacao);
}
