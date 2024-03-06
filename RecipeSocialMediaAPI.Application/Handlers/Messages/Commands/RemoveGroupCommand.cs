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

    public async Task Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
    {
        if (await _groupQueryRepository.GetGroupById(request.GroupId, cancellationToken) is null)
        {
            throw new GroupNotFoundException(request.GroupId);
        }

        bool isSuccessful = _groupPersistenceRepository.DeleteGroup(request.GroupId);

        if (!isSuccessful)
        {
            throw new GroupRemovalException(request.GroupId);
        }
    }
}
