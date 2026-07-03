using Boleiroffice.Application.Common.Models;
using Boleiroffice.Application.DTOs.Feed;

namespace Boleiroffice.Application.Interfaces.Services;

public interface IFeedService
{
    Task<PagedResult<FeedJogoResponse>> GetJogosAsync(PaginationParameters pagination, CancellationToken cancellationToken);
}
