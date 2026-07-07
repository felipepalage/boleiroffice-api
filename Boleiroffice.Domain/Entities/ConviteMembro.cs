using Boleiroffice.Domain.Common;

namespace Boleiroffice.Domain.Entities;

/// <summary>
/// Convite (link com token) para um novo usuário entrar numa empresa já cadastrada.
/// Permite vários logins por empresa sem reusar o CNPJ. Reutilizável até expirar.
/// </summary>
public class ConviteMembro : BaseEntity
{
    public Guid EmpresaId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime ExpiraEm { get; set; }

    public Empresa? Empresa { get; set; }
}
