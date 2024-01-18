using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveGroupCommand(string GroupId) : IRequest;

internal class RemoveGroupHandler : IRequestHandler<RemoveGroupCommand>
{
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IGroupQueryRepository _groupQueryRepository;

    public RemoveGroupHandler(IGroupPersistenceRepository groupPersistenceRepository, IGroupQueryRepository groupQueryRepository)
    {
        _groupPersistenceRepository = groupPersistenceRepository;
        _groupQueryRepository = groupQueryRepository;
    }

    public Task Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
    {
        if (_groupQueryRepository.GetGroupById(request.GroupId) is null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        bool isSuccessful = _groupPersistenceRepository.DeleteGroup(request.GroupId);

        return isSuccessful
            ? Task.CompletedTask
            : throw new GroupRemovalException(request.GroupId);
    }
}
