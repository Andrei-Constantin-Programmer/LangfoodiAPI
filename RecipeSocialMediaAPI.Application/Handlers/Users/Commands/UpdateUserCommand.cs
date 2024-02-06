using FluentValidation;
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

        var newPassword = request.Contract.Password is not null
            ? _cryptoService.Encrypt(request.Contract.Password)
            : existingUser.Password;

        IUserCredentials updatedUser = _userFactory.CreateUserCredentials(
            request.Contract.Id,
            existingUser.Account.Handler,
            request.Contract.UserName ?? existingUser.Account.UserName,
            request.Contract.Email ?? existingUser.Email,
            newPassword,
            request.Contract.ProfileImageId,
            existingUser.Account.AccountCreationDate
        );

        bool isSuccessful = _userPersistenceRepository.UpdateUser(updatedUser);

        return isSuccessful
            ? Task.CompletedTask 
            : throw new UserUpdateException($"Could not update user with id {request.Contract.Id}.");
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public UpdateUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.Contract.UserName)
            .Must(_userValidationService.ValidUserName)
            .When(x => x.Contract.UserName is not null);

        RuleFor(x => x.Contract.Email)
            .Must(_userValidationService.ValidEmail)
            .When(x => x.Contract.Email is not null);

        RuleFor(x => x.Contract.Password)
            .Must(_userValidationService.ValidPassword)
            .When(x => x.Contract.Password is not null);
    }
}
