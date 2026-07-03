namespace Boleiroffice.Domain.Entities;

public class Quadra
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? Estado { get; set; }
    public int? Capacidade { get; set; }
    public string? Cep { get; set; }
    public int TipoGrama { get; set; } // 0=Sintetico 1=Natural 2=Cimento 3=Borracha
    public bool Iluminacao { get; set; }
    public bool Vestiario { get; set; }
    public string? FotoUrl { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<AvaliacaoQuadra> Avaliacoes { get; set; } = [];
}
