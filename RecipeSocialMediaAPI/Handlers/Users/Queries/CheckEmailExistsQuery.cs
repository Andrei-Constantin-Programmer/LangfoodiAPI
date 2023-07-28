using MediatR;
using RecipeSocialMediaAPI.Core.Services;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

public record CheckEmailExistsQuery(string Email) : IRequest<bool>;

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
