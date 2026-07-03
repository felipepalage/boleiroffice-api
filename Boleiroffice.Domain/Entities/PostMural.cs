using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class PostMural : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string NomeAutor { get; set; } = string.Empty;
    public DateTime DataPublicacao { get; set; } = DateTime.UtcNow;

    public Empresa? Empresa { get; set; }
}
