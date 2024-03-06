using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
public record BlockConnectionCommand(string UserId, string ConnectionId) : IRequest;

internal class BlockConnectionHandler : IRequestHandler<BlockConnectionCommand>
{
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IUserPersistenceRepository _userPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public BlockConnectionHandler(IUserQueryRepository userQueryRepository, IUserPersistenceRepository userPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _userQueryRepository = userQueryRepository;
        _userPersistenceRepository = userPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task Handle(BlockConnectionCommand request, CancellationToken cancellationToken) {
        IUserCredentials user = (await _userQueryRepository.GetUserById(request.UserId, cancellationToken))
            ?? throw new UserNotFoundException($"User with id {request.UserId} does not exist");
        IConnection connection = (await _connectionQueryRepository.GetConnection(request.ConnectionId, cancellationToken))
            ?? throw new ConnectionNotFoundException($"Connection with id {request.ConnectionId} does not exist");

        if (user.Account.BlockConnection(connection.ConnectionId))
        {
            await _userPersistenceRepository.UpdateUser(user, cancellationToken);
        }
    }
}
