using MediatR;

namespace RecipeSocialMediaAPI.Validation;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IValidatableRequest
{

}

public interface IValidatableRequest
{

}