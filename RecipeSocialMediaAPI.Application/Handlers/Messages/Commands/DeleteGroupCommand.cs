using MediatR;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record DeleteGroupCommand(string GroupId) : IRequest<bool>;

internal class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    public Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
