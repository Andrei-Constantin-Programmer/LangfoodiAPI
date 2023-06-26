using FluentValidation;
using MediatR;
using OneOf;

namespace RecipeSocialMediaAPI.Validation;

public class ValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, OneOf<TResult, ValidationFailed>> where TRequest : notnull
{
    private readonly IValidator<TRequest> _validator;

    public ValidationBehavior(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<OneOf<TResult, ValidationFailed>> Handle(
        TRequest request,
        RequestHandlerDelegate<OneOf<TResult, ValidationFailed>> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        return await next();
    }
}
