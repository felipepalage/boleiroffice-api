using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class DisponibilidadeTime : BaseEntity
{
    public Guid TimeId { get; set; }
    public int DiaSemana { get; set; } // 0=Domingo..6=Sábado
    public TimeOnly Horario { get; set; }
    public string? NomeLocal { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string? EnderecoCompleto { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Time? Time { get; set; }
}
