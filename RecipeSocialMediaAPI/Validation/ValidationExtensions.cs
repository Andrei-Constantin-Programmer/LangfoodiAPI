using MediatR;
using OneOf;

namespace RecipeSocialMediaAPI.Validation;

public static class ValidationExtensions
{
    public static MediatRServiceConfiguration AddValidation<TRequest, TResponse>(
        this MediatRServiceConfiguration config) where TRequest : notnull
    {
        return config
            .AddBehavior<IPipelineBehavior<
                TRequest, 
                OneOf<TResponse, ValidationFailed>>, 
                ValidationBehavior<TRequest, TResponse>>();
    }
}
