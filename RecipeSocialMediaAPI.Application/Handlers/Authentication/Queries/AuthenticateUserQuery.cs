using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;
using RecipeSocialMediaAPI.Application.Services.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;

public record AuthenticateUserQuery(string Email, string Password) : IRequest<SuccessfulAuthenticationDTO>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, SuccessfulAuthenticationDTO>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserMapper _mapper;
    private readonly ICryptoService _cryptoService;
    private readonly IBearerTokenGeneratorService _bearerTokenGeneratorService;

    public AuthenticateUserHandler(
        IUserQueryRepository userQueryRepository,
        IUserMapper mapper,
        ICryptoService cryptoService,
        IBearerTokenGeneratorService bearerTokenGeneratorService)
    {
        _userQueryRepository = userQueryRepository;
        _mapper = mapper;
        _cryptoService = cryptoService;
        _bearerTokenGeneratorService = bearerTokenGeneratorService;
    }

    public async Task<SuccessfulAuthenticationDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        IUserCredentials user = await _userQueryRepository.GetUserByEmail(request.Email, cancellationToken)
                    ?? throw new UserNotFoundException($"No user found with handler/email {request.Email}");

        var successfulLogin = _cryptoService.ArePasswordsTheSame(request.Password, user.Password ?? string.Empty);
        if (!successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        var token = _bearerTokenGeneratorService.GenerateToken(user);

        return new SuccessfulAuthenticationDTO(_mapper.MapUserToUserDto(user), token);
    }
}
