using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

public record CheckUsernameExistsQuery(string Username) : IRequest<bool>;

internal class CheckUsernameExistsHandler : IRequestHandler<CheckUsernameExistsQuery, bool>
{
    private readonly IUserQueryRepository _userQueryRepository;

    public CheckUsernameExistsHandler(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }

    public Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userQueryRepository.GetUserByUsername(request.Username) is not null);
    }
}
