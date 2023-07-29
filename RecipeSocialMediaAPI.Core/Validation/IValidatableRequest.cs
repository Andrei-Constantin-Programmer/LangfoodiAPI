using MediatR;

namespace RecipeSocialMediaAPI.Core.Validation;

public interface IValidatableRequest<out TResponse> : IRequest<TResponse>, IValidatableRequest { }

public interface IValidatableRequestVoid : IRequest, IValidatableRequest { }

public interface IValidatableRequest { }