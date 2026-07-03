using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class ReacaoFeed : BaseEntity
{
    public Guid DesafioId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Desafio? Desafio { get; set; }
    public Empresa? Empresa { get; set; }
}
