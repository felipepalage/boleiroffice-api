using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Gol registrado numa partida do rachão, com autor e (opcional) assistência.
/// Alimenta o ranking de artilheiros e garçons do Modo Amistoso da empresa.
/// </summary>
public class GolAmistoso : BaseEntity
{
    public Guid PartidaAmistosoId { get; set; }
    public Guid EmpresaId { get; set; }
    public int TimeNumero { get; set; } // 1 ou 2
    public string AutorNome { get; set; } = string.Empty;
    public string? AssistenteNome { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public PartidaAmistoso? Partida { get; set; }
}
