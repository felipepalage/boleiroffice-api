namespace Boleiroffice.Application.DTOs.Ranking;

/// <summary>Contadores agregados exibidos na landing pública (prova social).</summary>
public sealed record EstatisticasPublicasResponse(int Empresas, int Times, int Jogos, int Gols);

/// <summary>Empresa que mais indicou outras (programa de indicação).</summary>
public sealed record IndicadorResponse(Guid EmpresaId, string EmpresaNome, int Total);
