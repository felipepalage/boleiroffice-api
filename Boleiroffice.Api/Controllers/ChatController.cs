using Boleiroffice.Api.Extensions;
using Boleiroffice.Application.DTOs.Chat;
using Boleiroffice.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boleiroffice.Api.Controllers;

[ApiController]
[Route("api/desafios/{desafioId:guid}/mensagens")]
[Authorize]
public sealed class ChatController : ControllerBase
{
    private readonly IChatService _service;

    public ChatController(IChatService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MensagemResponse>>> GetMessages(
        Guid desafioId, CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.GetByDesafioAsync(desafioId, currentUser, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MensagemResponse>> Send(
        Guid desafioId,
        [FromBody] EnviarMensagemRequest request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var result = await _service.SendAsync(desafioId, request, currentUser, cancellationToken);
        return Ok(result);
    }
}
