using MediatR;
using RecipeSocialMediaAPI.Services;

namespace RecipeSocialMediaAPI.Handlers.Users.Queries;

internal record CheckEmailExistsQuery(string Email) : IRequest<bool>;

internal class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
{
    private readonly IUserService _userService;

    public CheckEmailExistsHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userService.DoesEmailExist(request.Email));
    }
}
