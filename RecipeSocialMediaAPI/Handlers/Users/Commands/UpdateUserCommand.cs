using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.Validation;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

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
        request.UpdateUserContract.Password = _cryptoService.Encrypt(request.UpdateUserContract.Password);
        User updatedUser = new(
            request.UpdateUserContract.Id,
            request.UpdateUserContract.UserName,
            request.UpdateUserContract.Email,
            request.UpdateUserContract.Password
        );

        var result = _userRepository.UpdateUser(updatedUser);

        return result 
            ? Task.CompletedTask 
            : throw new UserNotFoundException();
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
