using FluentValidation;
using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Application.Validation;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record AddUserCommand(NewUserContract Contract) : IValidatableRequest<SuccessfulAuthenticationDTO>;

internal class AddUserHandler : IRequestHandler<AddUserCommand, SuccessfulAuthenticationDTO>
{
    private readonly IUserMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICryptoService _cryptoService;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;
    private readonly IBearerTokenGeneratorService _bearerTokenGeneratorService;

    public AddUserHandler(IUserMapper mapper, IDateTimeProvider dateTimeProvider, ICryptoService cryptoService, IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository, IBearerTokenGeneratorService bearerTokenGeneratorService)
    {
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _cryptoService = cryptoService;
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
        _bearerTokenGeneratorService = bearerTokenGeneratorService;
    }

    public async Task<SuccessfulAuthenticationDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        if ((await _userQueryRepository.GetUserByHandler(request.Contract.Handler, cancellationToken)) is not null)
        {
            throw new HandlerAlreadyInUseException(request.Contract.Handler);
        }

        if ((await _userQueryRepository.GetUserByUsername(request.Contract.UserName, cancellationToken)) is not null)
        {
            throw new UsernameAlreadyInUseException(request.Contract.UserName);
        }

        if ((await _userQueryRepository.GetUserByEmail(request.Contract.Email, cancellationToken)) is not null)
        {
            throw new EmailAlreadyInUseException(request.Contract.Email);
        }

        var encryptedPassword = _cryptoService.Encrypt(request.Contract.Password);
        IUserCredentials insertedUser = await _userPersistenceRepository
            .CreateUser(
                request.Contract.Handler,
                request.Contract.UserName,
                request.Contract.Email,
                encryptedPassword,
                _dateTimeProvider.Now,
                cancellationToken: cancellationToken);

        var token = _bearerTokenGeneratorService.GenerateToken(insertedUser);

        return new SuccessfulAuthenticationDTO(_mapper.MapUserToUserDto(insertedUser), token);
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    private readonly IUserValidationService _userValidationService;

    public AddUserCommandValidator(IUserValidationService userValidationService)
    {
        _userValidationService = userValidationService;

        RuleFor(x => x.Contract.Handler)
            .NotEmpty()
            .Must(_userValidationService.ValidHandler);

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
