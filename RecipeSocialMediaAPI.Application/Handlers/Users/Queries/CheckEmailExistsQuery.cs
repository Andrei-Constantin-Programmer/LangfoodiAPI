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

    public async Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
    {
        return await _userQueryRepository.GetUserByEmailAsync(request.Email, cancellationToken) is not null;
    }
}
