namespace Boleiroffice.Domain.Entities;

public class TorneioInscricao
{
    public Guid Id { get; set; }
    public Guid TorneioId { get; set; }
    public Torneio Torneio { get; set; } = null!;
    public Guid TimeId { get; set; }
    public Time Time { get; set; } = null!;
    public string? GrupoLetra { get; set; }
    public DateTime DataInscricao { get; set; } = DateTime.UtcNow;
}
