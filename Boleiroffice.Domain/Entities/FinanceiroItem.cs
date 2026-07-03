using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class FinanceiroItem : BaseEntity
{
    public Guid TimeId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = "Despesa";  // "Despesa" | "Receita"
    public string? Categoria { get; set; }  // "Campo", "Uniforme", "Equipamento", "Outro"
    public DateOnly DataVencimento { get; set; }
    public bool Pago { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public Time? Time { get; set; }
}
