using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.Common.Validation;
using Boleiroffice.Application.DTOs.Empresas;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;

    public EmpresaService(IEmpresaRepository empresaRepository, IImageStorageService imageStorageService, IMapper mapper)
    {
        _empresaRepository = empresaRepository;
        _imageStorageService = imageStorageService;
        _mapper = mapper;
    }

    public async Task<EmpresaResponse> CreateAsync(EmpresaCreateRequest request, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Cnpj, "CNPJ da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Bairro, "Bairro da empresa e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.Cidade, "Cidade da empresa e obrigatoria.");

        var normalizedCnpj = CnpjHelper.Normalize(request.Cnpj);
        if (!CnpjHelper.IsValid(normalizedCnpj))
        {
            throw new BusinessException("CNPJ invalido.");
        }

        if (await _empresaRepository.ExistsByNameAsync(request.Nome.Trim(), cancellationToken))
        {
            throw new BusinessException("Ja existe uma empresa com esse nome.");
        }

        if (await _empresaRepository.ExistsByCnpjAsync(normalizedCnpj, cancellationToken))
        {
            throw new BusinessException("Ja existe uma empresa com esse CNPJ.");
        }

        var empresa = new Empresa
        {
            Nome = request.Nome.Trim(),
            Cnpj = normalizedCnpj,
            Bairro = request.Bairro.Trim(),
            Cidade = request.Cidade.Trim(),
            LogoUrl = NormalizeUrl(request.LogoUrl),
            DataCriacao = DateTime.UtcNow
        };

        await _empresaRepository.AddAsync(empresa, cancellationToken);

        return _mapper.Map<EmpresaResponse>(empresa);
    }

    public async Task<EmpresaResponse> UpdateLogoAsync(Guid id, EmpresaImageUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Empresa invalida.");

        var empresa = await GetOwnedEmpresaAsync(id, currentUser, cancellationToken);
        var normalizedLogoUrl = NormalizeUrl(request.LogoUrl);
        if (string.Equals(empresa.LogoUrl, normalizedLogoUrl, StringComparison.OrdinalIgnoreCase))
        {
            return _mapper.Map<EmpresaResponse>(empresa);
        }

        var previousLogoUrl = empresa.LogoUrl;
        empresa.LogoUrl = normalizedLogoUrl;
        await _empresaRepository.UpdateAsync(empresa, cancellationToken);
        await _imageStorageService.DeleteIfManagedAsync(previousLogoUrl, cancellationToken);

        return _mapper.Map<EmpresaResponse>(empresa);
    }

    public async Task<EmpresaResponse> UploadLogoAsync(Guid id, Stream fileStream, string fileName, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Empresa invalida.");
        if (fileStream is null)
        {
            throw new BusinessException("Arquivo da empresa e obrigatorio.");
        }

        Guard.AgainstNullOrWhiteSpace(fileName, "Nome do arquivo e obrigatorio.");

        var empresa = await GetOwnedEmpresaAsync(id, currentUser, cancellationToken);
        var previousLogoUrl = empresa.LogoUrl;
        var uploadedLogoUrl = await _imageStorageService.SaveEmpresaLogoAsync(fileStream, fileName, cancellationToken);

        try
        {
            empresa.LogoUrl = uploadedLogoUrl;
            await _empresaRepository.UpdateAsync(empresa, cancellationToken);
        }
        catch
        {
            await _imageStorageService.DeleteIfManagedAsync(uploadedLogoUrl, cancellationToken);
            throw;
        }

        await _imageStorageService.DeleteIfManagedAsync(previousLogoUrl, cancellationToken);

        return _mapper.Map<EmpresaResponse>(empresa);
    }

    public async Task<EmpresaDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Empresa invalida.");

        var empresa = await _empresaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Empresa nao encontrada.");

        return _mapper.Map<EmpresaDetailsResponse>(empresa);
    }

    public async Task<PagedResult<EmpresaResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var empresas = await _empresaRepository.GetPagedAsync(normalized, cancellationToken);

        return empresas.Map(_mapper.Map<EmpresaResponse>);
    }

    private async Task<Empresa> GetOwnedEmpresaAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        if (id != currentUser.EmpresaId)
        {
            throw new BusinessException("Voce so pode atualizar a foto da sua propria empresa.");
        }

        return await _empresaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Empresa nao encontrada.");
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
