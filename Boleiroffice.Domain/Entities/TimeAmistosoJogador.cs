using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Vínculo (snapshot) de um jogador dentro de um time sorteado.
/// Guarda o nome no momento do sorteio para não depender do elenco atual.
/// </summary>
public class TimeAmistosoJogador : BaseEntity
{
    public Guid TimeAmistosoId { get; set; }
    public Guid? JogadorAmistosoId { get; set; }
    public string Nome { get; set; } = string.Empty;

    public TimeAmistoso? TimeAmistoso { get; set; }
}
