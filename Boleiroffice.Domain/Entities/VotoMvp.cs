using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class VotoMvp : BaseEntity
{
    public Guid DesafioId { get; set; }
    public Guid JogadorVotadoId { get; set; }
    public Guid VotantePorEmpresaId { get; set; }
    public DateTime DataVoto { get; set; } = DateTime.UtcNow;

    public Desafio? Desafio { get; set; }
    public Jogador? JogadorVotado { get; set; }
    public Empresa? VotantePorEmpresa { get; set; }
}
