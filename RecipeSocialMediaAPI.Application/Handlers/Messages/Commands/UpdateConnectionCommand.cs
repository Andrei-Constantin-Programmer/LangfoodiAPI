using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateConnectionCommand(UpdateConnectionContract Contract) : IRequest;

internal class UpdateConnectionHandler : IRequestHandler<UpdateConnectionCommand>
{
    private readonly IConnectionQueryRepository _connectionQueryRepository;
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    private readonly IUserQueryRepository _userQueryRepository;

    public UpdateConnectionHandler(IConnectionQueryRepository connectionQueryRepository, IConnectionPersistenceRepository connectionPersistenceRepository, IUserQueryRepository userQueryRepository)
    {
        _connectionQueryRepository = connectionQueryRepository;
        _connectionPersistenceRepository = connectionPersistenceRepository;
        _userQueryRepository = userQueryRepository;
    }

    public async Task Handle(UpdateConnectionCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = (await _userQueryRepository.GetUserByIdAsync(request.Contract.UserId1, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId1}");
        IUserAccount user2 = (await _userQueryRepository.GetUserByIdAsync(request.Contract.UserId2, cancellationToken))?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId2}");

        IConnection? connection = await _connectionQueryRepository.GetConnectionAsync(user1, user2, cancellationToken)
            ?? throw new ConnectionNotFoundException($"No connection found between users with ids {user1.Id} and {user2.Id}");

        var isValidConnectionStatus = Enum.TryParse(request.Contract.NewConnectionStatus, out ConnectionStatus newStatus);
        if (!isValidConnectionStatus)
        {
            throw new UnsupportedConnectionStatusException(request.Contract.NewConnectionStatus);
        }

        if (connection.Status == newStatus)
        {
            return;
        }

        connection.Status = newStatus;

        var isSuccessful = await _connectionPersistenceRepository.UpdateConnectionAsync(connection, cancellationToken);

        if (!isSuccessful)
        {
            throw new ConnectionUpdateException($"Could not update connection between users with ids {user1.Id} and {user2.Id}");
        }
    }
}
