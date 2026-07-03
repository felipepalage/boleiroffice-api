using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class Time : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public int Nivel { get; set; }
    public string BairroBase { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public int EscudoShape { get; set; } = 1;
    public string CorPrimaria { get; set; } = "#DC2626";
    public string CorSecundaria { get; set; } = "#111827";
    public string? Cep { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public int PenalidadesCount { get; set; } = 0;
    public DateTime? BloqueioAte { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Empresa? Empresa { get; set; }
    public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();
    public ICollection<Desafio> DesafiosCriados { get; set; } = new List<Desafio>();
    public ICollection<Desafio> DesafiosRecebidos { get; set; } = new List<Desafio>();
    public ICollection<GolPartida> GolsMarcados { get; set; } = new List<GolPartida>();
}
