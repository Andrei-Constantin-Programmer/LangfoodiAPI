﻿using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record UpdateUserCommand(UpdateUserContract Contract) : IValidatableRequest;

internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly ICryptoService _cryptoService;
    private readonly IUserFactory _userFactory;

    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public UpdateUserHandler(ICryptoService cryptoService, IUserFactory userFactory, IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _cryptoService = cryptoService;
        _userFactory = userFactory;
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        IUserCredentials existingUser = 
            _userQueryRepository.GetUserById(request.Contract.Id)
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.Id}");

        var encryptedPassword = _cryptoService.Encrypt(request.Contract.Password);
        IUserCredentials updatedUser = _userFactory.CreateUserCredentials(
            request.Contract.Id,
            existingUser.Account.Handler,
            request.Contract.UserName,
            request.Contract.Email,
            encryptedPassword,
            existingUser.Account.AccountCreationDate
        );

        bool isSuccessful = _userPersistenceRepository.UpdateUser(updatedUser);

        return isSuccessful
            ? Task.CompletedTask 
            : throw new Exception($"Could not update user with id {updatedUser.Account.Id}.");
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public UpdateUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.Contract.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.Contract.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.Contract.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
