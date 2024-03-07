using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
public record UnblockConnectionCommand(string UserId, string ConnectionId) : IRequest;

internal class UnblockConnectionHandler : IRequestHandler<UnblockConnectionCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public UnblockConnectionHandler(IUserQueryRepository userQueryRepository, IUserPersistenceRepository userPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
        _userPersistenceRepository = userPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task Handle(UnblockConnectionCommand request, CancellationToken cancellationToken) {
        IUserCredentials user = (await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken))
            ?? throw new UserNotFoundException($"User with id {request.UserId} does not exist");

        IConnection connection = (await _connectionQueryRepository.GetConnectionAsync(request.ConnectionId, cancellationToken))
            ?? throw new ConnectionNotFoundException($"Connection with id {request.ConnectionId} does not exist");

        if (user.Account.UnblockConnection(connection.ConnectionId))
        {
            await _userPersistenceRepository.UpdateUserAsync(user, cancellationToken);
        }
    }
}
