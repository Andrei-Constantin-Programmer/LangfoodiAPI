﻿using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record RemoveUserCommand(string EmailOrId) : IRequest;

internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public RemoveUserHandler(IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        var userId = ((await _userQueryRepository.GetUserByIdAsync(request.EmailOrId, cancellationToken))
            ?? (await _userQueryRepository.GetUserByEmailAsync(request.EmailOrId, cancellationToken))
            ?? throw new UserNotFoundException($"No user found with email/id {request.EmailOrId}")).Account.Id;

        bool isSuccessful = await _userPersistenceRepository.DeleteUserAsync(userId, cancellationToken);

        if (!isSuccessful)
        {
            throw new UserRemovalException(userId);
        }
    }
}
