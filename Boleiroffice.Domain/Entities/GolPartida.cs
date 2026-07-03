using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class GolPartida : BaseEntity
{
    public Guid DesafioId { get; set; }
    public Guid TimeId { get; set; }
    public string NomeAutor { get; set; } = string.Empty;
    public int QuantidadeGols { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Desafio? Desafio { get; set; }
    public Time? Time { get; set; }
}
