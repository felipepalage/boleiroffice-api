namespace Boleiroffice.Application.Common.Validation;

public static class CpfHelper
{
    public static string Normalize(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return string.Empty;
        }

        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    public static bool IsValid(string? cpf)
    {
        var normalized = Normalize(cpf);

        if (normalized.Length != 11)
        {
            return false;
        }

        if (normalized.Distinct().Count() == 1)
        {
            return false;
        }

        var numbers = normalized.Select(ch => ch - '0').ToArray();
        var firstWeights = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var secondWeights = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var firstDigit = CalculateDigit(numbers, firstWeights, 9);
        if (numbers[9] != firstDigit)
        {
            return false;
        }

        var secondDigit = CalculateDigit(numbers, secondWeights, 10);
        return numbers[10] == secondDigit;
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
