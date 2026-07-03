namespace Boleiroffice.Application.Common.Models;

public sealed class PaginationParameters
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public int Page { get; init; } = DefaultPage;
    public int PageSize { get; init; } = DefaultPageSize;
    public DateOnly? DataInicio { get; init; }

    public PaginationParameters Normalize()
    {
        var page = Page < 1 ? DefaultPage : Page;
        var pageSize = PageSize < 1 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);

        return new PaginationParameters
        {
            Page = page,
            PageSize = pageSize,
            DataInicio = DataInicio,
        };
    }
}
