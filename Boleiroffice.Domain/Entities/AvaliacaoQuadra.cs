namespace Boleiroffice.Domain.Entities;

public class AvaliacaoQuadra
{
    public Guid Id { get; set; }
    public Guid QuadraId { get; set; }
    public Quadra Quadra { get; set; } = null!;
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public int Nota { get; set; } // 1-5
    public string? Comentario { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
