namespace Boleiroffice.Domain.Entities;

public class Torneio
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Formato { get; set; } // 0=MataMata 1=Grupos
    public int Status { get; set; } // 0=Inscricoes 1=EmAndamento 2=Finalizado 3=Cancelado
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public Guid EmpresaOrganizadoraId { get; set; }
    public Empresa EmpresaOrganizadora { get; set; } = null!;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<TorneioInscricao> Inscricoes { get; set; } = [];
    public ICollection<PartidaTorneio> Partidas { get; set; } = [];
}
