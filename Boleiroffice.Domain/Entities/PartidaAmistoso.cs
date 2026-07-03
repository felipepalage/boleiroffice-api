using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Partida do rachão (Modo Amistoso). Time 1 x Time 2, com cronômetro e placar.
/// </summary>
public class PartidaAmistoso : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Time1Nome { get; set; } = "Time 1";
    public string Time2Nome { get; set; } = "Time 2";
    public int Time1Gols { get; set; }
    public int Time2Gols { get; set; }
    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataFim { get; set; }
    public int DuracaoSegundos { get; set; }
    public bool Finalizada { get; set; }

    public Empresa? Empresa { get; set; }
    public ICollection<GolAmistoso> Gols { get; set; } = new List<GolAmistoso>();
}
