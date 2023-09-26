using MediatR;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Queries;

public record CheckEmailExistsQuery(string Email) : IRequest<bool>;

internal class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
{
    private readonly IUserQueryRepository _userQueryRepository;

    public CheckEmailExistsHandler(IUserQueryRepository userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
    }

    public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userQueryRepository.GetUserByEmail(request.Email) is not null);
    }
}
