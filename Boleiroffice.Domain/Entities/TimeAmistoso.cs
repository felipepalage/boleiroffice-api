using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Time gerado pelo sorteio do Modo Amistoso. Cada sorteio substitui os times
/// anteriores da empresa (guardamos apenas o último sorteio).
/// </summary>
public class TimeAmistoso : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public DateTime DataSorteio { get; set; } = DateTime.UtcNow;

    public Empresa? Empresa { get; set; }
    public ICollection<TimeAmistosoJogador> Jogadores { get; set; } = new List<TimeAmistosoJogador>();
}
