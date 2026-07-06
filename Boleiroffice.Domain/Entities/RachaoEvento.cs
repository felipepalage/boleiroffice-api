using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Evento de rachão do dia: gera um link público onde os jogadores confirmam presença
/// com o nome. 2h antes do horário os times são sorteados automaticamente. Dado temporário.
/// </summary>
public class RachaoEvento : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime HorarioEvento { get; set; } // UTC — quando o rachão acontece
    public int NumeroTimes { get; set; } = 2;
    public bool SorteioFeito { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Empresa? Empresa { get; set; }
    public ICollection<RachaoConfirmacao> Confirmacoes { get; set; } = new List<RachaoConfirmacao>();
}
