using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Empresas;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/empresas")]
public sealed class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _empresaService;

    public EmpresasController(IEmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaResponse>> Create([FromBody] EmpresaCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await _empresaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [Authorize]
    [HttpPut("{id:guid}/logo")]
    public async Task<ActionResult<EmpresaResponse>> UpdateLogo(Guid id, [FromBody] EmpresaImageUpdateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _empresaService.UpdateLogoAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:guid}/logo/upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<EmpresaResponse>> UploadLogo(Guid id, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Selecione um arquivo de imagem." });
        }

        var currentUser = User.GetCurrentUser();
        await using var stream = file.OpenReadStream();
        var response = await _empresaService.UploadLogoAsync(id, stream, file.FileName, currentUser, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmpresaResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await _empresaService.GetPagedAsync(new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmpresaDetailsResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _empresaService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }
}
