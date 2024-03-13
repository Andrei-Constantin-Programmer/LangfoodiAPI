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

    public async Task<bool> Handle(CheckUsernameExistsQuery request, CancellationToken cancellationToken)
    {
        return await _userQueryRepository.GetUserByUsernameAsync(request.Username, cancellationToken) is not null;
    }
}
