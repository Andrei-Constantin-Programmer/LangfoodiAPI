using MediatR;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Authentication.Querries;

internal record AuthenticateUserQuery(UserDto User) : IRequest<bool>;

internal class AuthenticateUserHandler : IRequestHandler<AuthenticateUserQuery, bool>
{
    private readonly IUserService _userService;

    public AuthenticateUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<bool> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userService.DoesUserExist(request.User));
    }
}