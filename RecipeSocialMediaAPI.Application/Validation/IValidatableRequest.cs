using MediatR;

namespace RecipeSocialMediaAPI.Application.Validation;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IValidatableRequest { }

public interface IValidatableRequestVoid : IRequest, IValidatableRequest { }

public interface IValidatableRequest { }