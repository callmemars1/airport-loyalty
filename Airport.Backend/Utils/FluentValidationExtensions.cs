using FluentValidation.Results;

namespace Airport.Backend.Utils;

public static class FluentValidationExtensions
{
    public static IDictionary<string, string[]> ToDictionaryLower(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => ToCamelCase(g.Key),
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
    
    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}