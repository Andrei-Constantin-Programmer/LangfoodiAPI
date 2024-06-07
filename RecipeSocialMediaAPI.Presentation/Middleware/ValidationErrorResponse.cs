using FluentValidation;

namespace RecipeSocialMediaAPI.Presentation.Middleware;

public record ValidationErrorResponse
{
    public string Message { get; } = "Validation failed";
    public IEnumerable<string> Errors { get; }

    public ValidationErrorResponse(ValidationException validationException)
        : this(validationException.Errors
              .Select(error => $"Invalid {GetFormattedPropertyName(error.PropertyName)} with value '{error.AttemptedValue}'.")
              .Distinct())
    { }

    private ValidationErrorResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    private static string GetFormattedPropertyName(string propertyName)
    {
        const int containsPeriod = -1;
        int lastPeriodIndex = propertyName.LastIndexOf('.');

        return
            lastPeriodIndex == containsPeriod
            ? propertyName
            : propertyName[(lastPeriodIndex + 1)..];
    }
}
