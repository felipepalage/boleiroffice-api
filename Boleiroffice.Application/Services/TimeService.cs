using AutoMapper;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Times;
using Boleiroffice.Application.Exceptions;
using Boleiroffice.Application.Interfaces.Repositories;
using Boleiroffice.Application.Interfaces.Services;
using Boleiroffice.Domain.Entities;

namespace Boleiroffice.Application.Services;

public sealed class TimeService : ITimeService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;
    private readonly ITimeRepository _timeRepository;

    public TimeService(ITimeRepository timeRepository, IEmpresaRepository empresaRepository, IImageStorageService imageStorageService, IMapper mapper)
    {
        _timeRepository = timeRepository;
        _empresaRepository = empresaRepository;
        _imageStorageService = imageStorageService;
        _mapper = mapper;
    }

    public async Task<TimeResponse> CreateAsync(TimeCreateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(request.EmpresaId, "Empresa do time e obrigatoria.");
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do time e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.BairroBase, "Bairro base do time e obrigatorio.");
        Guard.AgainstInvalidLevel(request.Nivel, "Nivel do time deve estar entre 1 e 5.");

        if (currentUser.EmpresaId != request.EmpresaId)
        {
            throw new BusinessException("Voce so pode criar times para a sua propria empresa.");
        }

        var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaId, cancellationToken)
            ?? throw new NotFoundException("Empresa nao encontrada.");

        if (await _timeRepository.ExistsByNameWithinCompanyAsync(request.EmpresaId, request.Nome.Trim(), cancellationToken))
        {
            throw new BusinessException("Ja existe um time com esse nome nessa empresa.");
        }

        var time = new Time
        {
            Nome = request.Nome.Trim(),
            EmpresaId = request.EmpresaId,
            Empresa = empresa,
            Nivel = request.Nivel,
            BairroBase = request.BairroBase.Trim(),
            FotoUrl = NormalizeUrl(request.FotoUrl),
            EscudoShape = request.EscudoShape > 0 ? request.EscudoShape : 1,
            CorPrimaria = string.IsNullOrWhiteSpace(request.CorPrimaria) ? "#DC2626" : request.CorPrimaria.Trim(),
            CorSecundaria = string.IsNullOrWhiteSpace(request.CorSecundaria) ? "#111827" : request.CorSecundaria.Trim(),
            Cep = NormalizeUrl(request.Cep),
            Cidade = NormalizeUrl(request.Cidade),
            Estado = NormalizeUrl(request.Estado),
            DataCriacao = DateTime.UtcNow
        };

        await _timeRepository.AddAsync(time, cancellationToken);

        return _mapper.Map<TimeResponse>(time);
    }

    public async Task<TimeResponse> UpdateAsync(Guid id, TimeUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Nome, "Nome do time e obrigatorio.");
        Guard.AgainstNullOrWhiteSpace(request.BairroBase, "Bairro base do time e obrigatorio.");
        Guard.AgainstInvalidLevel(request.Nivel, "Nivel do time deve estar entre 1 e 5.");

        var time = await _timeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Time nao encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Voce so pode editar times da sua propria empresa.");

        if (!string.Equals(time.Nome, request.Nome.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            if (await _timeRepository.ExistsByNameWithinCompanyAsync(time.EmpresaId, request.Nome.Trim(), cancellationToken))
                throw new BusinessException("Ja existe um time com esse nome nessa empresa.");
        }

        time.Nome = request.Nome.Trim();
        time.Nivel = request.Nivel;
        time.BairroBase = request.BairroBase.Trim();
        time.EscudoShape = request.EscudoShape > 0 ? request.EscudoShape : 1;
        time.CorPrimaria = string.IsNullOrWhiteSpace(request.CorPrimaria) ? "#DC2626" : request.CorPrimaria.Trim();
        time.CorSecundaria = string.IsNullOrWhiteSpace(request.CorSecundaria) ? "#111827" : request.CorSecundaria.Trim();
        time.Cep = NormalizeUrl(request.Cep);
        time.Cidade = NormalizeUrl(request.Cidade);
        time.Estado = NormalizeUrl(request.Estado);

        await _timeRepository.UpdateAsync(time, cancellationToken);
        return _mapper.Map<TimeResponse>(time);
    }

    public async Task DeleteAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        var time = await _timeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Time nao encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
            throw new BusinessException("Voce so pode apagar times da sua propria empresa.");

        await _timeRepository.DeleteAsync(time, cancellationToken);
    }

    public async Task<TimeResponse> UpdateImageAsync(Guid id, TimeImageUpdateRequest request, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Time invalido.");

        var time = await GetOwnedTimeAsync(id, currentUser, cancellationToken);
        var normalizedFotoUrl = NormalizeUrl(request.FotoUrl);
        if (string.Equals(time.FotoUrl, normalizedFotoUrl, StringComparison.OrdinalIgnoreCase))
        {
            return _mapper.Map<TimeResponse>(time);
        }

        var previousFotoUrl = time.FotoUrl;
        time.FotoUrl = normalizedFotoUrl;
        await _timeRepository.UpdateAsync(time, cancellationToken);
        await _imageStorageService.DeleteIfManagedAsync(previousFotoUrl, cancellationToken);

        return _mapper.Map<TimeResponse>(time);
    }

    public async Task<TimeResponse> UploadImageAsync(Guid id, Stream fileStream, string fileName, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Time invalido.");
        if (fileStream is null)
        {
            throw new BusinessException("Arquivo do time e obrigatorio.");
        }

        Guard.AgainstNullOrWhiteSpace(fileName, "Nome do arquivo e obrigatorio.");

        var time = await GetOwnedTimeAsync(id, currentUser, cancellationToken);
        var previousFotoUrl = time.FotoUrl;
        var uploadedFotoUrl = await _imageStorageService.SaveTimeFotoAsync(fileStream, fileName, cancellationToken);

        try
        {
            time.FotoUrl = uploadedFotoUrl;
            await _timeRepository.UpdateAsync(time, cancellationToken);
        }
        catch
        {
            await _imageStorageService.DeleteIfManagedAsync(uploadedFotoUrl, cancellationToken);
            throw;
        }

        await _imageStorageService.DeleteIfManagedAsync(previousFotoUrl, cancellationToken);

        return _mapper.Map<TimeResponse>(time);
    }

    public async Task<TimeDetailsResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Guard.AgainstDefault(id, "Time invalido.");

        var time = await _timeRepository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException("Time nao encontrado.");

        return _mapper.Map<TimeDetailsResponse>(time);
    }

    public async Task<PagedResult<TimeResponse>> GetPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var normalized = pagination.Normalize();
        var times = await _timeRepository.GetPagedAsync(normalized, cancellationToken);

        return times.Map(_mapper.Map<TimeResponse>);
    }

    private async Task<Time> GetOwnedTimeAsync(Guid id, CurrentUser currentUser, CancellationToken cancellationToken)
    {
        var time = await _timeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Time nao encontrado.");

        if (time.EmpresaId != currentUser.EmpresaId)
        {
            throw new BusinessException("Voce so pode atualizar a foto dos times da sua empresa.");
        }

        return time;
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
