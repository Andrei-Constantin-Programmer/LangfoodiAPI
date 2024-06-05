using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;

public record AuthenticateUserQuery(string Email, string Password) : IRequest<SuccessfulAuthenticationDto>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, SuccessfulAuthenticationDto>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserMapper _mapper;
    private readonly IPasswordCryptoService _passwordCryptoService;
    private readonly IBearerTokenGeneratorService _bearerTokenGeneratorService;

    public AuthenticateUserHandler(
        IUserQueryRepository userQueryRepository,
        IUserMapper mapper,
        IPasswordCryptoService passwordCryptoService,
        IBearerTokenGeneratorService bearerTokenGeneratorService)
    {
        _userQueryRepository = userQueryRepository;
        _mapper = mapper;
        _passwordCryptoService = passwordCryptoService;
        _bearerTokenGeneratorService = bearerTokenGeneratorService;
    }

    public async Task<SuccessfulAuthenticationDto> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        IUserCredentials user = await _userQueryRepository.GetUserByEmailAsync(request.Email, cancellationToken)
                    ?? throw new UserNotFoundException($"No user found with email {request.Email}");

        var successfulLogin = _passwordCryptoService.ArePasswordsTheSame(request.Password, user.Password ?? string.Empty);
        if (!successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        var token = _bearerTokenGeneratorService.GenerateToken(user);

        return new SuccessfulAuthenticationDto(_mapper.MapUserToUserDto(user), token);
    }
}
