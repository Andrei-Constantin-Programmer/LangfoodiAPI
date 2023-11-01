using FluentValidation;
using Microsoft.Extensions.Options;

namespace RecipeSocialMediaAPI.Core.OptionValidators;

public class ValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    private readonly IServiceProvider _serviceProvider;

    private readonly string? _name;

    public ValidateOptions(IServiceProvider serviceProvider, string? name)
    {
        _serviceProvider = serviceProvider;
        _name = name;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (_name is not null
            && _name != name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        using var scope = _serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

        var result = validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var optionType = options.GetType().Name;
        var errors = new List<string>();

        foreach (var error in result.Errors)
        {
            errors.Add($"Validation failed for {optionType}.{error.PropertyName} with error: {error.ErrorMessage}");
        }

        return ValidateOptionsResult.Fail(errors);
    }
}
