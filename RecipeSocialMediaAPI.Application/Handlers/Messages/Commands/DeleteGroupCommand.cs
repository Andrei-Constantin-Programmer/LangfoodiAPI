using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record DeleteGroupCommand(string GroupId) : IRequest<bool>;

internal class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;

    public DeleteGroupHandler(IGroupPersistenceRepository groupPersistenceRepository)
    {
        _groupPersistenceRepository = groupPersistenceRepository;
    }

    public Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_groupPersistenceRepository.DeleteGroup(request.GroupId));
    }
}
