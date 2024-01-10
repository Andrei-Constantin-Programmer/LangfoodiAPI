using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateGroupCommand(UpdateGroupContract UpdateGroupContract) : IValidatableRequest<bool>;

internal class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, bool>
{
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public UpdateGroupHandler(IGroupQueryRepository groupQueryRepository, IGroupPersistenceRepository groupPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _groupQueryRepository = groupQueryRepository;
        _groupPersistenceRepository = groupPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Task<bool> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        Group group = _groupQueryRepository.GetGroupById(request.UpdateGroupContract.GroupId)
            ?? throw new GroupNotFoundException(request.UpdateGroupContract.GroupId);

        Group updatedGroup = new(
            groupId: request.UpdateGroupContract.GroupId,
            groupName: request.UpdateGroupContract.GroupName,
            groupDescription: request.UpdateGroupContract.GroupDescription,
            users: group.Users.ToList());

        var newUserList = request.UpdateGroupContract.UserIds
            .Select(userId => _userQueryRepository.GetUserById(userId)?.Account
                           ?? throw new UserNotFoundException(userId))
            .ToList();

        UpdateGroupUserList(updatedGroup, newUserList);

        return Task.FromResult(_groupPersistenceRepository.UpdateGroup(updatedGroup));
    }

    private static void UpdateGroupUserList(Group group, List<IUserAccount> newUserList)
    {
        for (var i = 0; i < group.Users.Count; i++)
        {
            var user = group.Users[i];
            if (!newUserList.Any(u => u.Id == user.Id))
            {
                group.RemoveUser(user);
                i--;
            }
        }

        foreach (var user in newUserList)
        {
            if (!group.Users.Any(u => u.Id == user.Id))
            {
                group.AddUser(user);
            }
        }
    }
}

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.UpdateGroupContract.GroupId)
            .NotEmpty();

        RuleFor(x => x.UpdateGroupContract.GroupName)
            .NotEmpty();
    }
}