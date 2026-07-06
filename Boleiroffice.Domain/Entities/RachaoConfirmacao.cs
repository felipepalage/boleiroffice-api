using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Confirmação de presença de um jogador num evento de rachão (feita pelo link público, sem login).
/// </summary>
public class RachaoConfirmacao : BaseEntity
{
    public Guid RachaoEventoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Empresa { get; set; }
    public string ChaveUnica { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public RachaoEvento? Evento { get; set; }
}
