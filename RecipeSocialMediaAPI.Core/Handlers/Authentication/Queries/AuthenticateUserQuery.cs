using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.Core.Cryptography.Interfaces;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.Core.DTO;
using RecipeSocialMediaAPI.Core.Exceptions;
using RecipeSocialMediaAPI.Core.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Domain.Entities;

namespace RecipeSocialMediaAPI.Core.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<UserDTO>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICryptoService _cryptoService;

    public AuthenticateUserHandler(IUserRepository userRepository, IMapper mapper, ICryptoService cryptoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _cryptoService = cryptoService;
    }

    public Task<UserDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        User? user = _userRepository.GetUserByUsername(request.UsernameOrEmail)
                    ?? _userRepository.GetUserByEmail(request.UsernameOrEmail);

        if (user is null)
        {
            throw new UserNotFoundException();
        }

        var successfulLogin = _cryptoService.ArePasswordsTheSame(request.Password, user?.Password ?? string.Empty);
        if (!successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        return Task.FromResult(_mapper.Map<UserDTO>(user));
    }
}
