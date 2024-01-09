using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract NewUserContract) : IValidatableRequest<UserDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, UserDTO>
{
    private readonly IUserMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICryptoService _cryptoService;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public AddUserHandler(IUserMapper mapper, IDateTimeProvider dateTimeProvider, ICryptoService cryptoService, IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _cryptoService = cryptoService;
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task<UserDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if (_userQueryRepository.GetUserByHandler(request.NewUserContract.Handler) is not null)
        {
            throw new HandlerAlreadyInUseException(request.NewUserContract.Handler);
        }

        if (_userQueryRepository.GetUserByUsername(request.NewUserContract.UserName) is not null)
        {
            throw new UsernameAlreadyInUseException(request.NewUserContract.UserName);
        }

        if (_userQueryRepository.GetUserByEmail(request.NewUserContract.Email) is not null)
        {
            throw new EmailAlreadyInUseException(request.NewUserContract.Email);
        }

        var encryptedPassword = _cryptoService.Encrypt(request.NewUserContract.Password);
        IUserCredentials insertedUser = _userPersistenceRepository
            .CreateUser(
                request.NewUserContract.Handler,
                request.NewUserContract.UserName,
                request.NewUserContract.Email,
                encryptedPassword,
                _dateTimeProvider.Now);

        return await Task.FromResult(_mapper.MapUserToUserDto(insertedUser));
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public AddUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.NewUserContract.Handler)
            .NotEmpty()
            .Must(_userValidationService.ValidHandler);

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
