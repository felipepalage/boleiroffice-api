using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class Temporada : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public bool Ativa { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
