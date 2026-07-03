using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class Empresa : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<Time> Times { get; set; } = new List<Time>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}