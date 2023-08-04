using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Domain;
using RecipeSocialMediaAPI.Core.Services.Interfaces;
using RecipeSocialMediaAPI.Core.Validation;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Commands;

public record UpdateUserCommand(UpdateUserContract UpdateUserContract) : IValidatableRequestVoid;

internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly ICryptoService _cryptoService;

    private readonly IUserRepository _userRepository;

    public UpdateUserHandler(ICryptoService cryptoService, IUserRepository userRepository)
    {
        _cryptoService = cryptoService;
        _userRepository = userRepository;
    }

    public Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var doesUserExist = _userRepository.GetUserById(request.UpdateUserContract.Id) is not null;
        if (!doesUserExist)
        {
            throw new UserNotFoundException();
        }

        var encryptedPassword = _cryptoService.Encrypt(request.UpdateUserContract.Password);
        User updatedUser = new(
            request.UpdateUserContract.Id,
            request.UpdateUserContract.UserName,
            request.UpdateUserContract.Email,
            encryptedPassword
        );

        var result = _userRepository.UpdateUser(updatedUser);

        return result 
            ? Task.CompletedTask 
            : throw new Exception($"Could not update user with id {updatedUser.Id}.");
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public UpdateUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.UpdateUserContract.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.UpdateUserContract.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.UpdateUserContract.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
