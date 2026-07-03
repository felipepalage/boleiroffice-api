using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Jogador do elenco do Modo Amistoso (rachão interno da empresa).
/// Não tem relação com a entidade Jogador (que pertence a um Time competitivo).
/// </summary>
public class JogadorAmistoso : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public bool PagouMensalidade { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Empresa? Empresa { get; set; }
}
