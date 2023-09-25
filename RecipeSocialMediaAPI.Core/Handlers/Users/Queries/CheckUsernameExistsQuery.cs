using MediatR;
using RecipeSocialMediaAPI.Application.Repositories;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

public record CheckUsernameExistsQuery(string Username) : IRequest<bool>;

internal class CheckUsernameExistsHandler : IRequestHandler<CheckUsernameExistsQuery, bool>
{
    private readonly IUserRepository _userRepository;

    public CheckUsernameExistsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userRepository.GetUserByUsername(request.Username) is not null);
    }
}
