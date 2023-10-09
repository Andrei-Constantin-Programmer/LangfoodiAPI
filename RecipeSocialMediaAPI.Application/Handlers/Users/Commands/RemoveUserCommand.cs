using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;

public record RemoveUserCommand(string EmailOrId) : IRequest;

internal class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;

    public RemoveUserHandler(IUserPersistenceRepository userPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _userPersistenceRepository = userPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public Task Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        var userId = (_userQueryRepository.GetUserById(request.EmailOrId)
            ?? _userQueryRepository.GetUserByEmail(request.EmailOrId)
            ?? throw new UserNotFoundException()).Account.Id;

        bool isSuccessful = _userPersistenceRepository.DeleteUser(userId);

        return isSuccessful
            ? Task.CompletedTask 
            : throw new Exception($"Could not remove user with id {userId}.");
    }
}
