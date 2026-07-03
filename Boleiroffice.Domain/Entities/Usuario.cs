using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public bool IsAdmin { get; set; } = false;

    public Empresa? Empresa { get; set; }
}
