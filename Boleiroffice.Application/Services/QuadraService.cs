using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Quadras;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class QuadraService : IQuadraService
{
    private readonly IQuadraRepository _quadraRepository;

    public QuadraService(IQuadraRepository quadraRepository)
    {
        _quadraRepository = quadraRepository;
    }

    public async Task<QuadraResponse> CreateAsync(QuadraCreateRequest request, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome da quadra é obrigatório.");
        Guard.AgainstNullOrWhiteSpace(request.Bairro, "Bairro é obrigatório.");
        Guard.AgainstNullOrWhiteSpace(request.Cidade, "Cidade é obrigatória.");

        var quadra = new Quadra
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Endereco = request.Endereco?.Trim() ?? string.Empty,
            Bairro = request.Bairro.Trim(),
            Cidade = request.Cidade.Trim(),
            Estado = string.IsNullOrWhiteSpace(request.Estado) ? null : request.Estado.Trim().ToUpper(),
            Cep = string.IsNullOrWhiteSpace(request.Cep) ? null : request.Cep.Replace("-", "").Trim(),
            Capacidade = request.Capacidade,
            TipoGrama = request.TipoGrama,
            Iluminacao = request.Iluminacao,
            Vestiario = request.Vestiario,
            DataCriacao = DateTime.UtcNow
        };

        await _quadraRepository.AddAsync(quadra, cancellationToken);
        return MapResponse(quadra);
    }

    public async Task<QuadraResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var quadra = await _quadraRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Quadra não encontrada.");
        return MapResponse(quadra);
    }

    public async Task<PagedResult<QuadraResponse>> GetPagedAsync(string? bairro, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _quadraRepository.GetPagedAsync(bairro, pagination, cancellationToken);
        return result.Map(MapResponse);
    }

    public async Task<QuadraResponse> AvaliarAsync(Guid id, AvaliacaoCreateRequest request, Guid empresaId, CancellationToken cancellationToken)
    {
        if (request.Nota < 1 || request.Nota > 5)
            throw new BusinessException("Nota deve ser entre 1 e 5.");

        var quadra = await _quadraRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Quadra não encontrada.");

        if (await _quadraRepository.JaAvaliouAsync(id, empresaId, cancellationToken))
            throw new BusinessException("Sua empresa já avaliou esta quadra.");

        var avaliacao = new AvaliacaoQuadra
        {
            Id = Guid.NewGuid(),
            QuadraId = id,
            EmpresaId = empresaId,
            Nota = request.Nota,
            Comentario = string.IsNullOrWhiteSpace(request.Comentario) ? null : request.Comentario.Trim(),
            DataCriacao = DateTime.UtcNow
        };

        await _quadraRepository.AddAvaliacaoAsync(avaliacao, cancellationToken);
        quadra = await _quadraRepository.GetByIdAsync(id, cancellationToken)!;
        return MapResponse(quadra!);
    }

    private static QuadraResponse MapResponse(Quadra q)
    {
        var avaliacoes = q.Avaliacoes
            .OrderByDescending(a => a.DataCriacao)
            .Select(a => new AvaliacaoResponse(a.Id, a.Empresa?.Nome ?? "—", a.Nota, a.Comentario, a.DataCriacao))
            .ToList();
        var notaMedia = avaliacoes.Count == 0 ? 0.0 : avaliacoes.Average(a => a.Nota);
        return new QuadraResponse(q.Id, q.Nome, q.Endereco, q.Bairro, q.Cidade, q.Estado, q.Cep,
            q.Capacidade, q.TipoGrama, q.Iluminacao, q.Vestiario, q.FotoUrl,
            Math.Round(notaMedia, 1), avaliacoes.Count, avaliacoes, q.DataCriacao);
    }
}
