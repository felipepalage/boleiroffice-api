using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Auth;
using Boleiroffice.Application.Interfaces.Services;

namespace Boleiroffice.Infrastructure.Services;

/// <summary>
/// Consulta o CNPJ na BrasilAPI (gratuita, sem chave). É best-effort: qualquer falha
/// retorna null e o cadastro segue com preenchimento manual.
/// </summary>
public sealed class CnpjLookupService : ICnpjLookupService
{
    private readonly HttpClient _http;

    public CnpjLookupService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CnpjInfoResponse?> ConsultarAsync(string cnpj, CancellationToken cancellationToken)
    {
        var normalized = CnpjHelper.Normalize(cnpj);
        if (!CnpjHelper.IsValid(normalized))
        {
            return null;
        }

        try
        {
            using var response = await _http.GetAsync($"https://brasilapi.com.br/api/cnpj/v1/{normalized}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<BrasilApiCnpj>(cancellationToken);
            if (payload is null)
            {
                return null;
            }

            return new CnpjInfoResponse(
                normalized,
                string.IsNullOrWhiteSpace(payload.RazaoSocial) ? null : payload.RazaoSocial.Trim(),
                string.IsNullOrWhiteSpace(payload.NomeFantasia) ? null : payload.NomeFantasia.Trim(),
                string.IsNullOrWhiteSpace(payload.Municipio) ? null : payload.Municipio.Trim(),
                string.IsNullOrWhiteSpace(payload.Bairro) ? null : payload.Bairro.Trim(),
                string.IsNullOrWhiteSpace(payload.Uf) ? null : payload.Uf.Trim());
        }
        catch
        {
            // Timeout, DNS, indisponibilidade da API etc. — não pode derrubar o fluxo de cadastro.
            return null;
        }
    }

    private sealed record BrasilApiCnpj
    {
        [JsonPropertyName("razao_social")] public string? RazaoSocial { get; init; }
        [JsonPropertyName("nome_fantasia")] public string? NomeFantasia { get; init; }
        [JsonPropertyName("municipio")] public string? Municipio { get; init; }
        [JsonPropertyName("bairro")] public string? Bairro { get; init; }
        [JsonPropertyName("uf")] public string? Uf { get; init; }
    }
}
