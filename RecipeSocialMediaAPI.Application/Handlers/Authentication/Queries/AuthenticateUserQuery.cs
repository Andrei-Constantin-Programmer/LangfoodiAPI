using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Application.Mappers.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;

public record AuthenticateUserQuery(string HandlerOrEmail, string Password) : IRequest<UserDTO>;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDTO>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserMapper _mapper;
    private readonly ICryptoService _cryptoService;

    public AuthenticateUserHandler(IUserQueryRepository userQueryRepository, IUserMapper mapper, ICryptoService cryptoService)
    {
        _userQueryRepository = userQueryRepository;
        _mapper = mapper;
        _cryptoService = cryptoService;
    }

    public Task<UserDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        IUserCredentials user = (_userQueryRepository.GetUserByEmail(request.HandlerOrEmail))
                    ?? throw new UserNotFoundException($"No user found with handler/email {request.HandlerOrEmail}");

        var successfulLogin = _cryptoService.ArePasswordsTheSame(request.Password, user.Password ?? string.Empty);

        return !successfulLogin 
            ? throw new InvalidCredentialsException() 
            : Task.FromResult(_mapper.MapUserToUserDto(user));
    }
}
