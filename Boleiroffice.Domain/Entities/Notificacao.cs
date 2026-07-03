namespace Boleiroffice.Domain.Entities;

public class Notificacao
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool Lida { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
