namespace Boleiroffice.Application.DTOs.Financeiro;

public record FinanceiroItemRequest(string Descricao, decimal Valor, string Tipo, string? Categoria, DateOnly DataVencimento);
public record FinanceiroItemResponse(Guid Id, Guid TimeId, string Descricao, decimal Valor, string Tipo, string? Categoria, DateOnly DataVencimento, bool Pago, DateTime DataCriacao);
public record FinanceiroSummaryResponse(decimal TotalReceitas, decimal TotalDespesas, decimal Saldo, IReadOnlyList<FinanceiroItemResponse> Itens);
