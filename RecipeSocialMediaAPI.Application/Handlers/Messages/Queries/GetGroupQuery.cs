using MediatR;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;

public record GetGroupQuery(string GroupId) : IRequest<GroupDTO?>;

internal class GetGroupHandler : IRequestHandler<GetGroupQuery, GroupDTO?>
{
    private readonly IGroupQueryRepository _groupQueryRepository;

    public GetGroupHandler(IGroupQueryRepository groupQueryRepository)
    {
        _groupQueryRepository = groupQueryRepository;
    }

    public Task<GroupDTO?> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        Group? group = _groupQueryRepository.GetGroupById(request.GroupId);

        return Task.FromResult(group is not null
            ? new GroupDTO(
                group.GroupId,
                group.GroupName,
                group.GroupDescription, 
                group.Users
                    .Select(user => user.Id)
                    .ToList())
            : null);
    }
}
