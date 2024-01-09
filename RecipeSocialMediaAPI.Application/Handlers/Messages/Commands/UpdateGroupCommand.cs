using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateGroupCommand(UpdateGroupContract UpdateGroupContract) : IRequest<bool>;

internal class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, bool>
{
    public Task<bool> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}