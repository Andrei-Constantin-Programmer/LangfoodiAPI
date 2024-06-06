using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record CreateGroupCommand(NewGroupContract Contract) : IRequest<GroupDto>;

internal class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupDto>
{
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public CreateGroupHandler(IGroupPersistenceRepository GroupPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _groupPersistenceRepository = GroupPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        string groupName = request.Contract.Name;
        string groupDesciption = request.Contract.Description;

        var userAccounts = (await Task.WhenAll(request.Contract.UserIds
            .Select(async userid => (await _userQueryRepository.GetUserByIdAsync(userid, cancellationToken))?.Account
                ?? throw new UserNotFoundException($"No user found with id {userid}"))))
            .ToList();

        Group createdGroup = await _groupPersistenceRepository
            .CreateGroupAsync(groupName, groupDesciption, userAccounts, cancellationToken);

        return new GroupDto(
            createdGroup.GroupId,
            createdGroup.GroupName,
            createdGroup.GroupDescription,
            createdGroup.Users.Select(user => user.Id).ToList());
    }
}
