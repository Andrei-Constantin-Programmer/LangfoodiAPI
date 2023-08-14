using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Core.Contracts;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Validation;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract NewUserContract) : IValidatableRequest<UserDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDTO>
{
    private readonly IMapper _mapper;
    private readonly ICryptoService _cryptoService;
    private readonly IUserRepository _userRepository;

    public AddUserHandler(IMapper mapper, ICryptoService cryptoService, IUserRepository userRepository)
    {
        _mapper = mapper;
        _cryptoService = cryptoService;
        _userRepository = userRepository;
    }

    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if (_userRepository.GetUserByUsername(request.NewUserContract.UserName) is not null)
        {
            throw new UsernameAlreadyInUseException(request.NewUserContract.UserName);
        }
        if (_userRepository.GetUserByEmail(request.NewUserContract.Email) is not null)
        {
            throw new EmailAlreadyInUseException(request.NewUserContract.Email);
        }

        var encryptedPassword = _cryptoService.Encrypt(request.NewUserContract.Password);
        User insertedUser = _userRepository.CreateUser(request.NewUserContract.UserName,
                                                       request.NewUserContract.Email,
                                                       encryptedPassword);

        return await Task.FromResult(_mapper.Map<UserDTO>(insertedUser));
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public AddUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.NewUserContract.UserName)
            .NotEmpty()
            .Must(_userValidationService.ValidUserName);

        RuleFor(x => x.NewUserContract.Email)
            .NotEmpty()
            .Must(_userValidationService.ValidEmail);

        RuleFor(x => x.NewUserContract.Password)
            .NotEmpty()
            .Must(_userValidationService.ValidPassword);
    }
}
