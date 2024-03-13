using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record ChangeUserRoleCommand(string UserId, string Role) : IRequest;

internal class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IUserPersistenceRepository _userPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public ChangeUserRoleHandler(IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        IUserCredentials user = await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException($"No user found with id {request.UserId}");

        var isValidUserRole = Enum.TryParse(request.Role, out UserRole role);

        if (!isValidUserRole)
        {
            throw new InvalidUserRoleException(request.Role);
        }

        user.Account.Role = role;
        await _userPersistenceRepository.UpdateUserAsync(user, cancellationToken);
    }
}
