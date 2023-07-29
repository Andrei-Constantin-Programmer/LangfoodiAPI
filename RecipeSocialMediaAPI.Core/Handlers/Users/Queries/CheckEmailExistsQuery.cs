using MediatR;
using RecipeSocialMediaAPI.DataAccess.Repositories.Interfaces;

namespace RecipeSocialMediaAPI.Core.Handlers.Users.Queries;

public record CheckEmailExistsQuery(string Email) : IRequest<bool>;

internal class CheckEmailExistsHandler : IRequestHandler<CheckEmailExistsQuery, bool>
{
    private readonly IUserRepository _userRepository;

    public CheckEmailExistsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_userRepository.GetUserByEmail(request.Email) is not null);
    }
}
