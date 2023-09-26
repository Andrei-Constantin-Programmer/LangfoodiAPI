using AutoMapper;
using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract NewUserContract) : IValidatableRequest<UserDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDTO>
{
    private readonly IMapper _mapper;
    private readonly ICryptoService _cryptoService;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public AddUserHandler(IMapper mapper, ICryptoService cryptoService, IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _mapper = mapper;
        _cryptoService = cryptoService;
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if (_userQueryRepository.GetUserByUsername(request.NewUserContract.UserName) is not null)
        {
            throw new UsernameAlreadyInUseException(request.NewUserContract.UserName);
        }

        if (_userQueryRepository.GetUserByEmail(request.NewUserContract.Email) is not null)
        {
            throw new EmailAlreadyInUseException(request.NewUserContract.Email);
        }

        var encryptedPassword = _cryptoService.Encrypt(request.NewUserContract.Password);
        User insertedUser = _userPersistenceRepository.CreateUser(request.NewUserContract.UserName,
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
