using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Handlers.Users.Queries;

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
