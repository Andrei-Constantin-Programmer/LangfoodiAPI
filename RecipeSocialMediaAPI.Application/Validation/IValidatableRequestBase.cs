using MediatR;

namespace RecipeSocialMediaAPI.Application.Validation;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IValidatableRequestBase { }

public interface IValidatableRequest : IRequest, IValidatableRequestBase { }

public interface IValidatableRequestBase { }