using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Times;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/times")]
public sealed class TimesController : ControllerBase
{
    private readonly ITimeService _timeService;

    public TimesController(ITimeService timeService)
    {
        _timeService = timeService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TimeResponse>> Create([FromBody] TimeCreateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _timeService.CreateAsync(request, currentUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TimeResponse>> Update(Guid id, [FromBody] TimeUpdateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _timeService.UpdateAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        await _timeService.DeleteAsync(id, currentUser, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id:guid}/foto")]
    public async Task<ActionResult<TimeResponse>> UpdateImage(Guid id, [FromBody] TimeImageUpdateRequest request, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _timeService.UpdateImageAsync(id, request, currentUser, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:guid}/foto/upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<TimeResponse>> UploadImage(Guid id, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Selecione um arquivo de imagem." });
        }

        var currentUser = User.GetCurrentUser();
        await using var stream = file.OpenReadStream();
        var response = await _timeService.UploadImageAsync(id, stream, file.FileName, currentUser, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TimeResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var response = await _timeService.GetPagedAsync(new PaginationParameters { Page = page, PageSize = pageSize }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TimeDetailsResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _timeService.GetByIdAsync(id, cancellationToken);
        return Ok(response);
    }
}
