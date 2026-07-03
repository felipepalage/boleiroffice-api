using Boleiroffice.Application.Exceptions;

namespace Boleiroffice.Application.Common.Models;

public static class Guard
{
    public static void AgainstDefault(Guid value, string message)
    {
        if (value == Guid.Empty)
        {
            throw new BusinessException(message);
        }
    }

    public static void AgainstInvalidLevel(int nivel, string message)
    {
        if (nivel < 1 || nivel > 5)
        {
            throw new BusinessException(message);
        }
    }

    public static void AgainstNullOrWhiteSpace(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessException(message);
        }
    }
}
