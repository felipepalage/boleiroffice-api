using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class ComentarioFeedService : IComentarioFeedService
{
    private readonly IComentarioFeedRepository _repo;
    private readonly IEmpresaRepository _empresaRepo;

    public ComentarioFeedService(IComentarioFeedRepository repo, IEmpresaRepository empresaRepo)
    {
        _repo = repo;
        _empresaRepo = empresaRepo;
    }

    public async Task<IReadOnlyList<ComentarioResponse>> GetByDesafioAsync(
        Guid desafioId, CancellationToken cancellationToken = default)
    {
        var items = await _repo.GetByDesafioAsync(desafioId, cancellationToken);
        return items.Select(ToResponse).ToList();
    }

    public async Task<ComentarioResponse> AddAsync(
        Guid desafioId, ComentarRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Conteudo) || request.Conteudo.Length > 500)
            throw new BusinessException("Comentário deve ter entre 1 e 500 caracteres.");

        var empresa = await _empresaRepo.GetByIdAsync(currentUser.EmpresaId, cancellationToken);
        var nomeEmpresa = empresa?.Nome ?? currentUser.Nome;

        var comentario = new ComentarioFeed
        {
            DesafioId = desafioId,
            EmpresaId = currentUser.EmpresaId,
            NomeEmpresa = nomeEmpresa,
            Conteudo = request.Conteudo.Trim(),
        };

        await _repo.AddAsync(comentario, cancellationToken);
        return ToResponse(comentario);
    }

    public async Task DeleteAsync(Guid comentarioId, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        var comentario = await _repo.GetByIdAsync(comentarioId, cancellationToken)
            ?? throw new NotFoundException("Comentário não encontrado.");

        if (comentario.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Você só pode excluir seus próprios comentários.");

        await _repo.DeleteAsync(comentario, cancellationToken);
    }

    private static ComentarioResponse ToResponse(ComentarioFeed c) => new(
        c.Id, c.DesafioId, c.EmpresaId, c.NomeEmpresa, c.Conteudo, c.DataComentario);
}
