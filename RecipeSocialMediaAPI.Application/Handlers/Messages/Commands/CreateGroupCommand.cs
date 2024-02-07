using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateGroupCommand(NewGroupContract Contract) : IRequest<GroupDTO>;

internal class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDTO>
{
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateGroupHandler(IGroupPersistenceRepository GroupPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _groupPersistenceRepository = GroupPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<GroupDTO> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        string groupName = request.Contract.Name;
        string groupDesciption = request.Contract.Description;
        List<IUserAccount> userAccounts = new List<IUserAccount>();
        List <string> userIds = request.Contract.UserIds;
        foreach (string userid in userIds)
        { 
            userAccounts.Add( _userQueryRepository
            .GetUserById(userid)?.Account
            ?? throw new UserNotFoundException($"No user found with id {userid}"));
        }



        Group createdGroup = _groupPersistenceRepository
            .CreateGroup(groupName, groupDesciption, userAccounts);

        return await Task.FromResult(new GroupDTO(
            createdGroup.GroupId,
            createdGroup.GroupName,
            createdGroup.GroupDescription,
            createdGroup.Users.Select(user => user.Id).ToList()));
    }
}
