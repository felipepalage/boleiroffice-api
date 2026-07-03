using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Mural;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class PostMuralService : IPostMuralService
{
    private readonly IPostMuralRepository _repo;
    private readonly IEmpresaRepository _empresaRepo;

    public PostMuralService(IPostMuralRepository repo, IEmpresaRepository empresaRepo)
    {
        _repo = repo;
        _empresaRepo = empresaRepo;
    }

    public async Task<IReadOnlyList<PostMuralResponse>> GetByEmpresaAsync(
        Guid empresaId, CancellationToken cancellationToken = default)
    {
        var posts = await _repo.GetByEmpresaAsync(empresaId, cancellationToken);
        return posts.Select(p => ToResponse(p, p.Empresa?.Nome ?? "Empresa")).ToList();
    }

    public async Task<PostMuralResponse> CreateAsync(
        Guid empresaId, PostMuralRequest request, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        if (empresaId != currentUser.EmpresaId)
            throw new BusinessException("Você só pode postar no mural da sua empresa.");

        var empresa = await _empresaRepo.GetByIdAsync(empresaId, cancellationToken)
            ?? throw new NotFoundException("Empresa não encontrada.");

        var post = new PostMural
        {
            EmpresaId = empresaId,
            Titulo = request.Titulo.Trim(),
            Conteudo = request.Conteudo.Trim(),
            NomeAutor = currentUser.Nome,
        };

        await _repo.AddAsync(post, cancellationToken);
        return ToResponse(post, empresa.Nome);
    }

    public async Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken = default)
    {
        var post = await _repo.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Post não encontrado.");

        if (post.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Você só pode excluir posts da sua empresa.");

        await _repo.DeleteAsync(post, cancellationToken);
    }

    private static PostMuralResponse ToResponse(PostMural p, string nomeEmpresa) => new(
        p.Id, p.EmpresaId, nomeEmpresa, p.Titulo, p.Conteudo, p.NomeAutor, p.DataPublicacao);
}
