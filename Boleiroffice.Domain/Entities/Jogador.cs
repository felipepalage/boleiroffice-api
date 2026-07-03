using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class Jogador : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Posicao { get; set; } = string.Empty;
    public int NumeroCamisa { get; set; }
    public Guid TimeId { get; set; }

    public Time? Time { get; set; }
    public ICollection<ConfirmacaoPresenca> ConfirmacoesPresenca { get; set; } = new List<ConfirmacaoPresenca>();
}
