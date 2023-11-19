using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateConnectionCommand(UpdateConnectionContract UpdateConnectionContract) : IRequest<bool>;

internal class UpdateConnectionHandler : IRequestHandler<UpdateConnectionCommand, bool>
{
    public Task<bool> Handle(UpdateConnectionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
