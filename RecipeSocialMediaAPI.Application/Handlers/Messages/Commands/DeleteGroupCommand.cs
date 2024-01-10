using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record DeleteGroupCommand(string GroupId) : IRequest<bool>;

internal class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, bool>
{
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;

    public DeleteGroupHandler(IGroupPersistenceRepository groupPersistenceRepository, IGroupQueryRepository groupQueryRepository)
    {
        _groupPersistenceRepository = groupPersistenceRepository;
        _groupQueryRepository = groupQueryRepository;
    }

    public Task<bool> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        if (_groupQueryRepository.GetGroupById(request.GroupId) is null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        return Task.FromResult(_groupPersistenceRepository.DeleteGroup(request.GroupId));
    }
}
