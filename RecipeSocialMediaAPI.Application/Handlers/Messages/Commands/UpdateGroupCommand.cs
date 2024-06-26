﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateGroupCommand(UpdateGroupContract Contract) : IValidatableRequest;

internal class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand>
{
    private readonly IGroupQueryRepository _groupQueryRepository;
    private readonly IGroupPersistenceRepository _groupPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly ILogger<UpdateGroupHandler> _logger;

    public UpdateGroupHandler(
        IGroupQueryRepository groupQueryRepository,
        IGroupPersistenceRepository groupPersistenceRepository,
        IUserQueryRepository userQueryRepository,
        ILogger<UpdateGroupHandler> logger)
    {
        _groupQueryRepository = groupQueryRepository;
        _groupPersistenceRepository = groupPersistenceRepository;
        _userQueryRepository = userQueryRepository;
        _logger = logger;
    }

    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        Group group = await _groupQueryRepository.GetGroupByIdAsync(request.Contract.GroupId, cancellationToken)
            ?? throw new GroupNotFoundException(request.Contract.GroupId);

        Group updatedGroup = new(
            groupId: request.Contract.GroupId,
            groupName: request.Contract.GroupName,
            groupDescription: request.Contract.GroupDescription,
            users: group.Users.ToList());

        var newUserList = (await Task.WhenAll(request.Contract.UserIds
            .Select(async userId
                => (await _userQueryRepository.GetUserByIdAsync(userId, cancellationToken))?.Account
                        ?? throw new UserNotFoundException(userId))))
            .ToList();

        UpdateGroupUserList(updatedGroup, newUserList);

        bool isSuccessful = false;
        if (updatedGroup.Users.Count == 0)
        {
            isSuccessful = await _groupPersistenceRepository.DeleteGroupAsync(updatedGroup, cancellationToken);
            _logger.LogInformation("Group with id {GroupId} was deleted due to all users quitting the group", group.GroupId);
        }
        else
        {
            isSuccessful = await _groupPersistenceRepository.UpdateGroupAsync(updatedGroup, cancellationToken);
        }

        if (!isSuccessful)
        {
            throw new GroupUpdateException($"Could not update group with id {group.GroupId}");
        }
    }

    private static void UpdateGroupUserList(Group group, List<IUserAccount> newUserList)
    {
        var i = 0;
        while (i < group.Users.Count)
        {
            var user = group.Users[i];
            if (!newUserList.Exists(u => u.Id == user.Id))
            {
                group.RemoveUser(user);
                continue;
            }

            i++;
        }

        foreach (var user in newUserList)
        {
            if (!group.Users.Exists(u => u.Id == user.Id))
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
        RuleFor(x => x.Contract.GroupId)
            .NotEmpty();

        RuleFor(x => x.Contract.GroupName)
            .NotEmpty();
    }
}