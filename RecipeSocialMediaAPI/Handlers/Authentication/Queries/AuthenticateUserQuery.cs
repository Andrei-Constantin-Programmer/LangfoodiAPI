using AutoMapper;
using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Exceptions;
using RecipeSocialMediaAPI.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Model;
using BCrypter = BCrypt.Net.BCrypt;

namespace RecipeSocialMediaAPI.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(string UsernameOrEmail, string Password) : IRequest<UserDTO>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, UserDTO>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthenticateUserHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public Task<UserDTO> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        User? user = _userRepository.GetUserByUsername(request.UsernameOrEmail)
                    ?? _userRepository.GetUserByEmail(request.UsernameOrEmail);

        if (user is null)
        {
            throw new UserNotFoundException();
        }

        var successfulLogin = BCrypter.Verify(request.Password, user?.Password);
        if (!successfulLogin)
        {
            throw new InvalidCredentialsException();
        }

        return Task.FromResult(_mapper.Map<UserDTO>(user));
    }
}
