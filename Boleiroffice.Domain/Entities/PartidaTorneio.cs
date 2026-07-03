namespace Boleiroffice.Domain.Entities;

public class PartidaTorneio
{
    public Guid Id { get; set; }
    public Guid TorneioId { get; set; }
    public Torneio Torneio { get; set; } = null!;
    public Guid TimeMandanteId { get; set; }
    public Time TimeMandante { get; set; } = null!;
    public Guid? TimeVisitanteId { get; set; }
    public Time? TimeVisitante { get; set; }
    public int? PlacarMandante { get; set; }
    public int? PlacarVisitante { get; set; }
    public string Fase { get; set; } = string.Empty; // "Grupos/A", "Oitavas", "Quartas", "Semi", "Final"
    public int Status { get; set; } // 0=Agendada 1=Realizada 2=Walkover
    public DateOnly? DataJogo { get; set; }
    public TimeOnly? HoraJogo { get; set; }
    public string? Local { get; set; }
}
