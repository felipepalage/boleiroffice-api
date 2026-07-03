using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class ConfirmacaoPresenca : BaseEntity
{
    public Guid JogadorId { get; set; }
    public Guid DesafioId { get; set; }
    public bool Confirmado { get; set; }

    public Jogador? Jogador { get; set; }
    public Desafio? Desafio { get; set; }
}
