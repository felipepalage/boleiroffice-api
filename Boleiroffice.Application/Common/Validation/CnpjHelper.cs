namespace Boleiroffice.Application.Common.Validation;

public static class CnpjHelper
{
    public static string Normalize(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            return string.Empty;
        }

        return new string(cnpj.Where(char.IsDigit).ToArray());
    }

    public static bool IsValid(string? cnpj)
    {
        var normalized = Normalize(cnpj);

        if (normalized.Length != 14)
        {
            return false;
        }

        if (normalized.Distinct().Count() == 1)
        {
            return false;
        }

        var numbers = normalized.Select(ch => ch - '0').ToArray();
        var firstWeights = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var secondWeights = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var firstDigit = CalculateDigit(numbers, firstWeights, 12);
        if (numbers[12] != firstDigit)
        {
            return false;
        }

        var secondDigit = CalculateDigit(numbers, secondWeights, 13);
        return numbers[13] == secondDigit;
    }

    private static int CalculateDigit(IReadOnlyList<int> numbers, IReadOnlyList<int> weights, int length)
    {
        var sum = 0;

        for (var index = 0; index < length; index++)
        {
            sum += numbers[index] * weights[index];
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}