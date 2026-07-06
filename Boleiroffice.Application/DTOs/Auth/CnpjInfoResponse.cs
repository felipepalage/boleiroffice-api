namespace Boleiroffice.Application.DTOs.Auth;

/// <summary>Dados públicos de uma empresa retornados pela consulta de CNPJ (BrasilAPI).</summary>
public sealed record CnpjInfoResponse(
    string Cnpj,
    string? RazaoSocial,
    string? NomeFantasia,
    string? Cidade,
    string? Bairro,
    string? Uf);
