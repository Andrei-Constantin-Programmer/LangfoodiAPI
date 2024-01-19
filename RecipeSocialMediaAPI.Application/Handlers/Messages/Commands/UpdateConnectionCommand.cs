using MediatR;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record UpdateConnectionCommand(UpdateConnectionContract Contract) : IRequest<bool>;

internal class UpdateConnectionHandler : IRequestHandler<UpdateConnectionCommand, bool>
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

    public Task<bool> Handle(UpdateConnectionCommand request, CancellationToken cancellationToken)
    {
        IUserAccount user1 = _userQueryRepository.GetUserById(request.Contract.UserId1)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId1}");
        IUserAccount user2 = _userQueryRepository.GetUserById(request.Contract.UserId2)?.Account
            ?? throw new UserNotFoundException($"No user found with id {request.Contract.UserId2}");

        IConnection? connection = _connectionQueryRepository.GetConnection(user1, user2);

        if (connection is null)
        {
            return Task.FromResult(false);
        }

        var isValidConnectionStatus = Enum.TryParse(request.Contract.NewConnectionStatus, out ConnectionStatus newStatus);
        if (!isValidConnectionStatus)
        {
            throw new ConnectionUpdateException($"Could not map {request.Contract.NewConnectionStatus} to {typeof(ConnectionStatus)}");
        }

        if (connection.Status == newStatus)
        {
            return Task.FromResult(true);
        }

        connection.Status = newStatus;
            
        return Task.FromResult(_connectionPersistenceRepository.UpdateConnection(connection));
    }
}
