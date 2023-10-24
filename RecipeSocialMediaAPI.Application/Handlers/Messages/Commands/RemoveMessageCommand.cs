using MediatR;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveMessageCommand(string Id) : IRequest<bool>;