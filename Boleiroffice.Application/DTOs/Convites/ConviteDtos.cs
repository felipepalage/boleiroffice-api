namespace Boleiroffice.Application.DTOs.Convites;

public sealed record ConviteResponse(string Token, DateTime ExpiraEm);

public sealed record ConviteInfoResponse(string EmpresaNome);

public sealed record AceitarConviteRequest(string Nome, string Email, string Senha);
