using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Validation;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateMessageCommand(UpdateMessageContract UpdateMessageContract) : IValidatableRequest<UpdateMessageCommand>;