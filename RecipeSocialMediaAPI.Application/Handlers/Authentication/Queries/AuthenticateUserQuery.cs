using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.Application.Cryptography.Interfaces;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Authentication.Querries;

public record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<UserDTO>;

public class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDTO>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IMapper _mapper;
    private readonly ICryptoService _cryptoService;

    public AuthenticateUserHandler(IUserQueryRepository userQueryRepository, IMapper mapper, ICryptoService cryptoService)
    {
        _userQueryRepository = userQueryRepository;
        _mapper = mapper;
        _cryptoService = cryptoService;
    }

    public Task<UserDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        User? user = (_userQueryRepository.GetUserByUsername(request.UsernameOrEmail)
                    ?? _userQueryRepository.GetUserByEmail(request.UsernameOrEmail)) 
                    ?? throw new UserNotFoundException();

        var successfulLogin = _cryptoService.ArePasswordsTheSame(request.Password, user?.Password ?? string.Empty);

        return !successfulLogin 
            ? throw new InvalidCredentialsException() 
            : Task.FromResult(_mapper.Map<UserDTO>(user));
    }
}
