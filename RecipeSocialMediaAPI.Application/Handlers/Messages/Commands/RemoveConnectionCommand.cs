using MediatR;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;

namespace RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;

public record RemoveConnectionCommand(string ConnectionId) : IRequest;

internal class RemoveConnectionHandler : IRequestHandler<RemoveConnectionCommand>
{
    private readonly IConnectionPersistenceRepository _connectionPersistenceRepository;
    private readonly IConnectionQueryRepository _connectionQueryRepository;

    public RemoveConnectionHandler(IConnectionPersistenceRepository connectionPersistenceRepository, IConnectionQueryRepository connectionQueryRepository)
    {
        _connectionPersistenceRepository = connectionPersistenceRepository;
        _connectionQueryRepository = connectionQueryRepository;
    }

    public async Task Handle(RemoveConnectionCommand request, CancellationToken cancellationToken)
    {
        IConnection connection = await _connectionQueryRepository.GetConnectionAsync(request.ConnectionId, cancellationToken)
            ?? throw new ConnectionNotFoundException($"No Connection found with id {request.ConnectionId}");

        await _connectionPersistenceRepository.DeleteConnectionAsync(connection, cancellationToken);
    }
}
