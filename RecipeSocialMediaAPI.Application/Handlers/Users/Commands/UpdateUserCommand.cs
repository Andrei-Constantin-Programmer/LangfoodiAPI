﻿using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record UpdateUserCommand(UpdateUserContract Contract) : IValidatableRequest;

internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IPasswordCryptoService _passwordCryptoService;
    private readonly IUserFactory _userFactory;

    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public UpdateUserHandler(
        IPasswordCryptoService passwordCryptoService,
        IUserFactory userFactory,
        IUserPersistenceRepository userPersistenceRepository,
        IUserQueryRepository userQueryRepository)
    {
        _passwordCryptoService = passwordCryptoService;
        _userFactory = userFactory;
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        IUserCredentials existingUser = await _userQueryRepository.GetUserByIdAsync(request.Contract.Id, cancellationToken)
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.Id}");

        var newPassword = request.Contract.Password is not null
            ? _passwordCryptoService.Encrypt(request.Contract.Password)
            : existingUser.Password;

        IUserCredentials updatedUser = _userFactory.CreateUserCredentials(
            request.Contract.Id,
            existingUser.Account.Handler,
            request.Contract.UserName ?? existingUser.Account.UserName,
            request.Contract.Email ?? existingUser.Email,
            newPassword,
            request.Contract.ProfileImageId ?? existingUser.Account.ProfileImageId,
            existingUser.Account.AccountCreationDate,
            existingUser.Account.PinnedConversationIds.ToList(),
            existingUser.Account.BlockedConnectionIds.ToList(),
            existingUser.Account.Role
        );

        bool isSuccessful = await _userPersistenceRepository.UpdateUserAsync(updatedUser, cancellationToken);

        if (!isSuccessful)
        {
            throw new UserUpdateException($"Could not update user with id {request.Contract.Id}.");
        }
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IUserValidationService userValidationService)
    {
        RuleFor(x => x.Contract.UserName)
            .Must(userValidationService.ValidUserName!)
            .When(x => x.Contract.UserName is not null);

        RuleFor(x => x.Contract.Email)
            .Must(userValidationService.ValidEmail!)
            .When(x => x.Contract.Email is not null);

        RuleFor(x => x.Contract.Password)
            .Must(userValidationService.ValidPassword!)
            .When(x => x.Contract.Password is not null);
    }
}
