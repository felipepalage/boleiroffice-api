using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class ComentarioFeed : BaseEntity
{
    public Guid DesafioId { get; set; }
    public Guid EmpresaId { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataComentario { get; set; } = DateTime.UtcNow;

    public Desafio? Desafio { get; set; }
    public Empresa? Empresa { get; set; }
}
