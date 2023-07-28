using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Model;
using RecipeSocialMediaAPI.Services;
using RecipeSocialMediaAPI.Services.Interfaces;
using RecipeSocialMediaAPI.Validation;

namespace RecipeSocialMediaAPI.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract NewUserContract) : IValidatableRequest<UserDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDTO>
{
    private readonly IUserValidationService _userValidationService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ICryptoService _cryptoService;

    private readonly IUserRepository _userRepository;

    public AddUserHandler(IUserValidationService userValidationService, IUserService userService, IMapper mapper, ICryptoService cryptoService, IUserRepository userRepository)
    {
        _userValidationService = userValidationService;
        _userService = userService;
        _mapper = mapper;
        _cryptoService = cryptoService;
        _userRepository = userRepository;
    }

    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if(_userService.DoesUsernameExist(request.NewUserContract.UserName))
        {
            throw new UsernameAlreadyInUseException(request.NewUserContract.UserName);
        }
        if (_userService.DoesEmailExist(request.NewUserContract.Email))
        {
            throw new EmailAlreadyInUseException(request.NewUserContract.Email);
        }

        request.NewUserContract.Password = _cryptoService.Encrypt(request.NewUserContract.Password);
        User insertedUser = _userRepository.CreateUser(request.NewUserContract.UserName,
                                                       request.NewUserContract.Email,
                                                       request.NewUserContract.Password);

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
