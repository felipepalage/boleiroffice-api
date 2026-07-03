using System.Text;
using System.Text.Json;
using Boleiroffice.Application.DTOs.Ia;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Boleiroffice.Infrastructure.Services;

/// <summary>
/// Narração/resumo de partidas via Claude (Anthropic Messages API).
/// A chave é lida de ANTHROPIC_API_KEY (env do backend) — nunca versionada.
/// </summary>
public sealed class AiService : IAiService
{
    // Modelo mais barato para geração simples de texto.
    private const string Model = "claude-haiku-4-5";
    private const string Endpoint = "https://api.anthropic.com/v1/messages";

    // HttpClient estático (instância única) — evita esgotar sockets e não exige AddHttpClient.
    private static readonly HttpClient Http = new();

    private readonly string? _apiKey;

    public AiService(IConfiguration configuration)
    {
        _apiKey = configuration["ANTHROPIC_API_KEY"];
    }

    public async Task<NarracaoResponse> GerarNarracaoAsync(NarracaoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new BusinessException("Narração por IA não está configurada (defina ANTHROPIC_API_KEY no servidor).");

        var artilheiros = request.Artilheiros is { Count: > 0 }
            ? string.Join(", ", request.Artilheiros)
            : "nenhum gol registrado";

        var prompt =
            "Você é um narrador de futebol animado e bem-humorado. Escreva uma narração curta " +
            "(2 a 4 frases, em português do Brasil) celebrando o resultado de uma pelada/rachão entre empresas. " +
            "Seja empolgado mas natural. Não invente dados além dos fornecidos.\n\n" +
            $"Partida: {request.Titulo}\n" +
            $"Placar: {request.Placar}\n" +
            $"Artilheiros: {artilheiros}\n" +
            (string.IsNullOrWhiteSpace(request.Contexto) ? "" : $"Contexto: {request.Contexto}\n");

        var payload = JsonSerializer.Serialize(new
        {
            model = Model,
            max_tokens = 400,
            messages = new[] { new { role = "user", content = prompt } }
        });

        using var httpReq = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        httpReq.Headers.Add("x-api-key", _apiKey);
        httpReq.Headers.Add("anthropic-version", "2023-06-01");

        HttpResponseMessage resp;
        try
        {
            resp = await Http.SendAsync(httpReq, cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new BusinessException("Não foi possível falar com o serviço de IA agora. Tente novamente.");
        }

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        if (!resp.IsSuccessStatusCode)
            throw new BusinessException("A IA não conseguiu gerar a narração agora. Tente novamente.");

        using var doc = JsonDocument.Parse(json);
        var texto = string.Empty;
        if (doc.RootElement.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
        {
            foreach (var block in content.EnumerateArray())
            {
                if (block.TryGetProperty("type", out var type) && type.GetString() == "text")
                {
                    texto = block.GetProperty("text").GetString() ?? string.Empty;
                    break;
                }
            }
        }

        return new NarracaoResponse(texto.Trim());
    }
}
